namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[Serializable]
	public abstract class TypeRepTemplate : TranslationBase, IEquatable<TypeRepTemplate>
	{
		private string _variant = "";
		// Translation Variant
		[XmlAttribute("variant")]
		[System.ComponentModel.DefaultValueAttribute("")]
		public string Variant { 
			get
			{
				if (_variant == null)
				{
					_variant = "";
				}
				return _variant;
			}
			set
			{
				_variant = value;
			}
		}

		// Type Name
		[XmlElement("Name")]
		public string TypeName { get; set; }

		private string[] _typeParams = null;
		[XmlArrayItem("Name")]
		public string[] TypeParams { 
			get
			{
				if (_typeParams == null)
				{
					TypeParams = new string[0];
				}
				return _typeParams;
			}
			set
			{
				// First time that TypeParams is set then create InstantiatedTypes as corresponding list of TypeVars 
				if (value != null && InstantiatedTypes == null)
				{
					InstantiatedTypes = new TypeRepTemplate[value.Length];
					for (int i = 0; i < value.Length; i++)
					{
						InstantiatedTypes[i] = new TypeVarRepTemplate(value[i]);
					}
				}
				_typeParams = value;
			}
		}

		[XmlIgnore]
		public TypeRepTemplate[] InstantiatedTypes { get; set; }

		[XmlIgnore]
		public Dictionary<string,TypeRepTemplate> TyVarMap 
		{ get
		{ 
			Dictionary<string,TypeRepTemplate> ret = new Dictionary<string,TypeRepTemplate>(TypeParams.Length);
			for (int i = 0; i < TypeParams.Length; i++)
			{
				ret[TypeParams[i]] = InstantiatedTypes[i];
			}
			return ret;
		}
         
		}

		protected string[] _uses;
		// Path to use when resolving types
		[XmlArrayItem("Use")]
		public string[] Uses { get{return _uses;} set { _uses = value; updateUsesWithNamespace(); } }

		// Aliases for namespaces
		[XmlArrayItem("Alias")]
		public AliasRepTemplate[] Aliases { get; set; }

		protected List<CastRepTemplate> _casts = null;
		[XmlArrayItem("Cast")]
		public virtual List<CastRepTemplate> Casts {
			get {
				if (_casts == null)
					_casts = new List<CastRepTemplate> ();
				return _casts;
			}
		}

		private string[] _inherits;
		[XmlArrayItem("Type")]
		public string[] Inherits { 
			get { 
				if (_inherits == null)
				{
					_inherits = new string[] { "System.Object" };
				}
				return _inherits; 
			}
			set {
				if (value != null) {
					_inherits= new string[value.Length];
					for (int i = 0; i < value.Length; i++) {
						_inherits[i] = (value[i] != null ? value[i].Replace("<","*[").Replace(">","]*") : null);
					}
				}
				else {
					_inherits = null;
				}
			}
		}

		// Client can set IsExplicitNull.  If so then null IsA anytype 
		private bool _isExplicitNull = false;
		[XmlIgnore]
		public bool IsExplicitNull
		{
			get
			{
				return _isExplicitNull;
			}
			set
			{
				_isExplicitNull = value;
			}
		}

		// Set if value is wrapped in a RefSupport object (used for ref and out params) 
		private bool _isWrapped = false;
		[XmlIgnore]
		public bool IsWrapped
		{
			get
			{
				return _isWrapped;
			}
			set
			{
				_isWrapped = value;
			}
		}

		// Client can set _isUnboxedType.  If so then we know the expression / type is unboxed 
		private bool _isUnboxedType = false;
		[XmlIgnore]
		public bool IsUnboxedType
		{
			get
			{
				return _isUnboxedType;
			}
			set
			{
				_isUnboxedType = value;
			}
		}

		private String _boxedJava = null;
		public String BoxedJava {
			get
			{
				if (String.IsNullOrEmpty(_boxedJava))
				{
					_boxedJava = Java; 
				}
				return _boxedJava;
			}
			set
			{
				_boxedJava = value;
			}
		}

		// True if we have a separate representation for boxed and unboxed versions
		// (true for primitive types like int)
		private bool hasBoxedRep = false;
		[XmlAttribute("has_boxed_rep")]
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool HasBoxedRep
		{
			get
			{
				return hasBoxedRep;
			}
			set
			{
				hasBoxedRep = value;
			}
		}

		[XmlIgnore]
		public string BoxExpressionTemplate
		{
			get
			{
				return (String.IsNullOrEmpty(BoxedJava) ? "" : "((" + BoxedJava + ")${expr})");
			}
		}

		// Equivalent to "this is TypeVarRepTemplate"
		[XmlIgnore]
		public virtual bool IsTypeVar
		{
			get
			{
				return (this is TypeVarRepTemplate);
			}
		}
		// Equivalent to "this is UnknownRepTemplate"
		[XmlIgnore]
		public virtual bool IsUnknownType
		{
			get
			{
				return (this is UnknownRepTemplate);
			}
		}

		// Ugly, keep a copy of the tree. Its convenient if these are passed around with the type
		private object _tree = null;
		[XmlIgnore]
		public object Tree
		{
			get
			{
				return _tree;
			}
			set
			{
				_tree = value;
			}
		}

		public TypeRepTemplate()
			: base()
		{
			TypeName = null;
			Uses = null;
			Aliases = null;
		}

		protected TypeRepTemplate(string typeName)
			: this()
		{
			TypeName = typeName;
		}

		protected TypeRepTemplate(TypeRepTemplate copyFrom)
			:base(null, copyFrom)
		{
			if (!String.IsNullOrEmpty(copyFrom.TypeName)) 
			{
				TypeName = copyFrom.TypeName;
			}

			int len = 0;
			if (copyFrom.TypeParams != null)
			{
				len = copyFrom.TypeParams.Length;
				TypeParams = new String[len];
				for (int i = 0; i < len; i++)
				{
					TypeParams[i] = copyFrom.TypeParams[i];
				}
			}

			if (copyFrom.InstantiatedTypes != null)
			{
				len = copyFrom.InstantiatedTypes.Length;
				InstantiatedTypes = new TypeRepTemplate[len];
				for (int i = 0; i < len; i++)
				{
					InstantiatedTypes[i] = copyFrom.InstantiatedTypes[i];
				}
			}

			if (copyFrom.Uses != null)
			{
				len = copyFrom.Uses.Length;
				Uses = new String[len];
				for (int i = 0; i < len; i++)
				{
					Uses[i] = copyFrom.Uses[i];
				}
			}

			if (copyFrom.Aliases != null)
			{
				len = copyFrom.Aliases.Length;
				Aliases = new AliasRepTemplate[len];
				for (int i = 0; i < len; i++)
				{
					Aliases[i] = new AliasRepTemplate(copyFrom.Aliases[i]);
				}
			}

			foreach (CastRepTemplate c in copyFrom.Casts)
			{
				Casts.Add(new CastRepTemplate(this, c));
			}
         
			if (copyFrom.Inherits != null)
			{
				len = copyFrom.Inherits.Length;
				Inherits = new String[len];
				for (int i = 0; i < len; i++)
				{
					Inherits[i] = copyFrom.Inherits[i];
				}
			}

			IsExplicitNull = copyFrom.IsExplicitNull;
			IsUnboxedType = copyFrom.IsUnboxedType;
			Variant = copyFrom.Variant;
			BoxedJava = copyFrom.BoxedJava;
			HasBoxedRep = copyFrom.HasBoxedRep;
			Tree = copyFrom.Tree;
		}

		protected TypeRepTemplate(string tName, string[] tParams, string[] usePath, AliasRepTemplate[] aliases, string[] imports, string javaTemplate)
			: base(imports, javaTemplate)
		{
			TypeName = tName;
			TypeParams = tParams;
			Uses = usePath;
			Aliases = aliases;
		}

		public void updateUsesWithNamespace()
		{
			int ni = -1;
			if (!String.IsNullOrEmpty(TypeName) && (ni = TypeName.LastIndexOf('.')) > 0)
			{
				List<String> namespaces = null;
				String ns1 = TypeName.Substring(0, ni); 
				while (true)
				{
					if (namespaces == null)
					{
						if (_uses == null || Array.IndexOf(_uses,ns1)<0)
						{
							namespaces = new List<string>(_uses);
							namespaces.Add(ns1);
						}
					}
					else
					{
						if (!namespaces.Contains(ns1)) namespaces.Add(ns1);
					}
                  
					ni = ns1.LastIndexOf('.');
					if (ni <= 0) break;
					ns1 = ns1.Substring(0, ni);
				}
				if (namespaces != null)
					_uses = namespaces.ToArray();
			}
		}
		public override string mkJava() {
			string ret = String.Empty;
			if (TypeName != null && TypeName != String.Empty) {
				ret = TypeName.Substring(TypeName.LastIndexOf('.') + 1);
				if (TypeParams != null && TypeParams.Length > 0)
				{
					ret += mkTypeParams(TypeParams);
				}
			}
			return ret;
		}

		public override string[] mkImports() {
			if (TypeName !=  null) {
				return new string[] {TypeName};
			}
			else {
				return null;
			}
		}
             
		protected Dictionary<string,TypeRepTemplate> mkTypeMap(ICollection<TypeRepTemplate> args) { 
			Dictionary<string,TypeRepTemplate> ret = new Dictionary<string,TypeRepTemplate>();
			if (args == null)
			{
				return ret;
			}
			if (args.Count == TypeParams.Length)
			{
				int i = 0;
				foreach (TypeRepTemplate sub in args)
				{   
					ret[TypeParams[i]] = sub;   
					i++;
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException("Incorrect number of type arguments supplied when instantiating generic type");
			}
			return ret;
		}
        
		// Make a copy of 'this' and instantiate type variables with types and type variables.
                
		public abstract TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args);

                
		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (Casts != null)
			{
				foreach(CastRepTemplate c in Casts)
				{
					c.Apply(args);
				}
			}
			if (Inherits != null)
			{
				for(int i = 0; i < Inherits.Length; i++)
				{
					Inherits[i] = TemplateUtilities.SubstituteInType(Inherits[i],args);
				}
			}
			InstantiatedTypes = new TypeRepTemplate[InstantiatedTypes.Length];
			if (args != null) {
				for (int i = 0; i < TypeParams.Length; i++)
				{
					InstantiatedTypes[i] = args[TypeParams[i]];
				}
			}
			BoxedJava = TemplateUtilities.SubstituteInType(BoxedJava,args);
			base.Apply(args);
		}

		public ResolveResult Resolve(String name, IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			ResolveResult res = Resolve(name, args, AppEnv, false);
			if (TemplateUtilities.DO_IMPLICIT_CASTS &&  res == null) res = Resolve(name, args, AppEnv, true);
			return res;
		}
		// Resolve a method call (name and arg types)
		public virtual ResolveResult Resolve(String name, IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			if (Inherits != null)
			{
				foreach (String b in Inherits)
				{
					TypeRepTemplate baseType = BuildType(b, AppEnv);
					if (baseType != null)
					{
						ResolveResult ret = baseType.Resolve(name,args,AppEnv,implicitCast);
						if (ret != null)
							return ret;
					}
				}
			}
			return null;
		}
		public ResolveResult Resolve(String name, bool forWrite, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			ResolveResult res = Resolve(name, forWrite, AppEnv, false);
			if (TemplateUtilities.DO_IMPLICIT_CASTS && res == null) res = Resolve(name, forWrite, AppEnv, true);
			return res;
		}
		// Resolve a field or property access
		public virtual ResolveResult Resolve(String name, bool forWrite, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			if (Inherits != null)
			{
				foreach (String b in Inherits)
				{
					TypeRepTemplate baseType = BuildType(b, AppEnv);
					if (baseType != null)
					{
						ResolveResult ret = baseType.Resolve(name, forWrite, AppEnv,implicitCast);
						if (ret != null)
							return ret;
					}
				}
			}
			return null;
		}

		public ResolveResult ResolveIndexer(IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			ResolveResult res = ResolveIndexer(args, AppEnv, false);
			if (TemplateUtilities.DO_IMPLICIT_CASTS && res == null) res = ResolveIndexer(args, AppEnv, true);
			return res;
		}
		// Resolve a indexer call (arg types)
		public virtual ResolveResult ResolveIndexer(IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			if (Inherits != null)
			{
				foreach (String b in Inherits)
				{
					TypeRepTemplate baseType = BuildType(b, AppEnv);
					if (baseType != null)
					{
						ResolveResult ret = baseType.ResolveIndexer(args,AppEnv,implicitCast);
						if (ret != null)
							return ret;
					}
				}
			}
			return null;
		}

		// Resolve a cast from this type to castTo
		public virtual ResolveResult ResolveCastTo(TypeRepTemplate castTo, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			if (Casts != null)
			{
				foreach (CastRepTemplate c in Casts)
				{
					if (c.To != null)
					{
						// Is this a cast from us?
						TypeRepTemplate fromTy = null;
						if (c.From != null)
						{
							fromTy = BuildType(c.From, AppEnv);
						}
						if (c.From == null || (fromTy != null && fromTy.TypeName == TypeName))
						{
							// cast from us
							TypeRepTemplate toTy = BuildType(c.To, AppEnv);
							if ((toTy.IsA(castTo, AppEnv,false) && castTo.IsA(toTy, AppEnv,false)) || (toTy.IsA(castTo, AppEnv,true) && castTo.IsA(toTy, AppEnv,true)) )
							{
								ResolveResult res = new ResolveResult();
								res.Result = c;
								res.ResultType = toTy;
								return res;
							}
						}
					}
				}
			}
			if (Inherits != null)
			{
				foreach (String b in Inherits)
				{
					TypeRepTemplate baseType = BuildType(b, AppEnv);
					if (baseType != null)
					{
						ResolveResult ret = baseType.ResolveCastTo(castTo,AppEnv);
						if (ret != null)
							return ret;
					}
				}
			}
			return null;
		}

		// Resolve a cast to this type from castFrom
		public virtual ResolveResult ResolveCastFrom(TypeRepTemplate castFrom, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			if (Casts != null)
			{
				foreach (CastRepTemplate c in Casts)
				{
					if (c.From != null)
					{
						// Is this a cast to us?
						TypeRepTemplate toTy = null;
						if (c.To != null)
						{
							toTy = BuildType(c.To, AppEnv);
						}
						if (c.To == null || (toTy != null && toTy.TypeName == TypeName))
						{
							// cast to us
							TypeRepTemplate fromTy = BuildType(c.From, AppEnv);
							if ((castFrom.IsA(fromTy, AppEnv,false) && fromTy.IsA(castFrom, AppEnv,false)) || (castFrom.IsA(fromTy, AppEnv,true) && fromTy.IsA(castFrom, AppEnv,true)))
							{
								ResolveResult res = new ResolveResult();
								res.Result = c;
								res.ResultType = toTy;
								return res;
							}
						}
					}
				}
			}
			return null;
		}

		public virtual ResolveResult ResolveIterable(DirectoryHT<TypeRepTemplate> AppEnv)
		{
			if (Inherits != null)
			{
				foreach (String b in Inherits)
				{
					TypeRepTemplate baseType = BuildType(b, AppEnv);
					if (baseType != null)
					{
						ResolveResult ret = baseType.ResolveIterable(AppEnv);
						if (ret != null)
							return ret;
					}
				}
			}
			return null;
		}

		private List<string> primitiveTypes = null;
		private List<string> PrimitiveTypes
		{
			get
			{
				if (primitiveTypes == null)
				{
					primitiveTypes = new List<string>();
					primitiveTypes.Add("System.Boolean");
					primitiveTypes.Add("System.Byte");
					primitiveTypes.Add("System.Char");
					primitiveTypes.Add("System.Decimal");
					primitiveTypes.Add("System.Single");
					primitiveTypes.Add("System.Double");
					primitiveTypes.Add("System.Int32");
					primitiveTypes.Add("System.Int64");
					primitiveTypes.Add("System.SByte");
					primitiveTypes.Add("System.Int16");
					primitiveTypes.Add("System.String");
					primitiveTypes.Add("System.UInt32");
					primitiveTypes.Add("System.UInt64");
					primitiveTypes.Add("System.UInt16");
					primitiveTypes.Add("System.Void");

					primitiveTypes.Add("System.Enum");

					primitiveTypes.Add("long");
					primitiveTypes.Add("int");
					primitiveTypes.Add("short");
					primitiveTypes.Add("byte");

					primitiveTypes.Add("ulong");
					primitiveTypes.Add("uint");
					primitiveTypes.Add("ushort");
					primitiveTypes.Add("sbyte");

					primitiveTypes.Add("double");
					primitiveTypes.Add("float");
				}
				return primitiveTypes;
			}
		}

		public bool IsObject(DirectoryHT<TypeRepTemplate> AppEnv)
		{
			/*
           * Test part
           */
			bool debug = true;
			string type = this.TypeName.Split('?')[0];

			bool found = PrimitiveTypes.Contains(type);
			if (!found && Inherits!=null)
			{
				foreach (String ibase in Inherits)
				{
					TypeRepTemplate tbase = BuildType(ibase, AppEnv, new UnknownRepTemplate(ibase));
					if (!tbase.IsObject(AppEnv))
					{
						found = true;
						Console.WriteLine("FOUND : " + ibase + " in "+ type);
					}
				}
			}
			if (debug)
			{
				string[] typeSplit = type.Split('.');

				List<string> weirdFinals = new List<string>();
				weirdFinals.Add("APPLY");
				weirdFinals.Add("INDEXER");

				string inheritsString = "";
				foreach (string inherit in this.Inherits)
					inheritsString += " | " + inherit;

				if ((weirdFinals.Contains(typeSplit[typeSplit.Length - 1]) || typeSplit.Length == 1) && type.StartsWith("_"))
				{
					Console.WriteLine("TYPE : " + type);
					Console.WriteLine(found ? "I've found " + type + " in the list !" : "Nope, sorry, not in the list : " + type + " but this is its inherits : " + inheritsString);
				}
			}

			else
			{
				Console.WriteLine("TYPE : " + type);
				Console.WriteLine(found ? "I've found " + type + " in the list !" : "Nope, sorry, not in the list : " + type);

			}
			return !found;
		}
      
		Dictionary<String, String[]> implicitCasts = new Dictionary<string, string[]>()
		{
			{"System.SByte" , new String[] {"System.Int16","System.Int32","System.Int64","System.Float","System.Double","System.Decimal"}},
			{"System.Byte" , new String[] {"System.Int16","System.UInt16","System.Int32","System.UInt32","System.Int64","System.UInt64","System.Float","System.Double","System.Decimal"}},
			{"System.Int16" , new String[] {"System.Int32","System.Int64","System.Float","System.Double","System.Decimal"}},
			{"System.UInt16" , new String[] {"System.Int32","System.UInt32","System.Int64","System.UInt64","System.Float","System.Double","System.Decimal"}},
			{"System.Int32" , new String[] {"System.Int64","System.Float","System.Double","System.Decimal"}},
			{"System.UInt32" , new String[] {"System.Int64","System.UInt64","System.Float","System.Double","System.Decimal"}},
			{"System.Int64" , new String[] {"System.Float","System.Double","System.Decimal"}},
			{"System.UInt64" , new String[] {"System.Float","System.Double","System.Decimal"}},
			{"System.Char" , new String[] {"System.UInt16","System.Int32","System.UInt32","System.Int64","System.UInt64","System.Float","System.Double","System.Decimal"}},
			{"System.Float" , new String[] {"System.Double","System.Decimal"}},
		};
		// Returns true if other is a subclass, or implements our interface
		public virtual bool IsA(TypeRepTemplate other, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			return IsA(other, AppEnv, false);
		}
		public virtual bool IsA (TypeRepTemplate other, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast) {
			if (this.IsExplicitNull) 
			{
				return true;
			}

			/*
         * We avoid the nullable types
         */
			string otherType = other.TypeName.Split('?')[0];
			string thisType = this.TypeName.Split('?')[0];

			/*
           * We TEMPORARILY use this
           * 
           * We remove namespaces from types before to test the equality
           * (to avoid foo.bar.Foobar != Foobar)
           *

         string[] tempOtherType = otherType.Split('.');
         string[] tempThisType = thisType.Split('.');

         otherType = tempOtherType[tempOtherType.Length - 1];
         thisType = tempThisType[tempThisType.Length - 1];
          */
			if (otherType == thisType)
			{
				// See if generic arguments 'match'
				if (InstantiatedTypes != null && other.InstantiatedTypes != null && InstantiatedTypes.Length == other.InstantiatedTypes.Length)
				{
					bool isA = true;
					for (int i = 0; i < InstantiatedTypes.Length; i++)
					{
						if (!InstantiatedTypes[i].IsA(other.InstantiatedTypes[i],AppEnv,false))
						{
							isA = false;
							break;
						}
					}
					return isA;
				}
				else
				{
					// might be equal if they both represent "nothing"
					return (InstantiatedTypes == null && (other.InstantiatedTypes == null || other.InstantiatedTypes.Length == 0)) ||
					       (other.InstantiatedTypes == null && (InstantiatedTypes == null || InstantiatedTypes.Length == 0));
				}
			}
			else if (Inherits != null)
			{
				foreach (String ibase in Inherits)
				{
					TypeRepTemplate tbase = BuildType(ibase, AppEnv, new UnknownRepTemplate(ibase));
					if (tbase.IsA(other,AppEnv,implicitCast))
					{
						return true;
					}
				}
			}
			//Implicit cast : Stripped down version, returns the first that matches
			if (TemplateUtilities.DO_IMPLICIT_CASTS && implicitCast && this.Inherits != null && Array.IndexOf(this.Inherits, "System.Number") >= 0 && other.Inherits != null && Array.IndexOf(other.Inherits, "System.Number") >= 0)
			{
				String[] implicitCastTypes;
				if (implicitCasts.TryGetValue(this.TypeName, out implicitCastTypes) && Array.IndexOf(implicitCastTypes, other.TypeName)>=0) return true;
			}
			return false;
		}

		private class ParseResult<T>
		{
			public ParseResult(List<T> inParses, String inStr)
			{
				Parses = inParses;
				RemainingStr = inStr;
			}
			public List<T> Parses;
			public String RemainingStr;
		}

		// returns a list of type reps from a string representation:
		// <qualified.type.name>(*[<type>, <type>, ...]*)?([])*, .....
		private ParseResult<TypeRepTemplate> buildTypeList(string typeList, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			List<TypeRepTemplate> types = new List<TypeRepTemplate>();
			string typeStr = typeList.TrimStart();
			bool moreTypes = true;
			while (moreTypes)
			{
				// get type name from the start
				int nameEnd = typeStr.IndexOfAny(new char[] { '*','[',']',','});    
				string typeName = typeStr.Substring(0,(nameEnd >= 0 ? nameEnd : typeStr.Length)).TrimEnd();
				typeStr = typeStr.Substring(typeName.Length).TrimStart();

				// build basetype
				TypeRepTemplate typeRep = null;

				// Is it a type var?
				foreach (string p in TypeParams) {
					if (p == typeName)
					{
						typeRep = new TypeVarRepTemplate(typeName);
						break;
					}    
				}   
				if (typeRep == null)
				{
					// Not a type var, look for a type
					List<TypeRepTemplate> tyArgs = new List<TypeRepTemplate>();

					// Do we have type arguments?
					if (typeStr.Length > 0 && typeStr.StartsWith("*["))
					{
						// isolate type arguments
						ParseResult<TypeRepTemplate> args = buildTypeList(typeStr.Substring(2), AppEnv);
						tyArgs = args.Parses;
						typeStr = args.RemainingStr.TrimStart();
						if (typeStr.StartsWith("]*"))
						{
							typeStr = typeStr.Substring(2).TrimStart();
						}
						else
						{
							throw new Exception("buildTypeList: Cannot parse " + types);
						}
					}
					/*
                //Add Uses
                List<String> namespaces = new List<string>(this.Uses);
                String ns1=this.TypeName;
                while (true)
                {
                    int ni = ns1.LastIndexOf('.');
                    if (ni <= 0) break;
                    ns1 = ns1.Substring(0,ni);
                    if (!namespaces.Contains(ns1)) namespaces.Add(ns1);
                }
               typeRep = AppEnv.Search(namespaces.ToArray(), typeName + (tyArgs.Count > 0 ? "'" + tyArgs.Count.ToString() : ""), new UnknownRepTemplate(typeName));*/
					typeRep = AppEnv.Search(this.Uses, typeName + (tyArgs.Count > 0 ? "'" + tyArgs.Count.ToString() : ""), new UnknownRepTemplate(typeName));
					if (!typeRep.IsUnknownType && tyArgs.Count > 0)
					{
						typeRep = typeRep.Instantiate(tyArgs);
					}
				}
         
				// Take care of arrays ....
				while (typeStr.StartsWith("[]"))
				{
					TypeRepTemplate arrayType = AppEnv.Search("System.Array'1", new UnknownRepTemplate("System.Array'1"));
					typeRep = arrayType.Instantiate(new TypeRepTemplate[] { typeRep });
					typeStr = typeStr.Substring(2).TrimStart();
				}
				types.Add(typeRep);
				moreTypes = typeStr.StartsWith(",");
				if (moreTypes)
				{
					typeStr = typeStr.Substring(1).TrimStart();
				}
			} 
			return new ParseResult<TypeRepTemplate>(types, typeStr);
		}
		
		// Builds a type rep from a string representation
		// "type_name"
		// "<type>[]"
		// "<type>[<type>, ...]"
		public TypeRepTemplate BuildType(TypeRepRef typeRep, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			return BuildType(typeRep.Type, AppEnv, null);
		}
		public TypeRepTemplate BuildType(string typeRep, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			return BuildType(typeRep, AppEnv, null);
		}

		public TypeRepTemplate BuildType(TypeRepRef typeRep, DirectoryHT<TypeRepTemplate> AppEnv, TypeRepTemplate def)
		{
			return BuildType(typeRep.Type, AppEnv, def);
		}

		public TypeRepTemplate BuildType(string typeRep, DirectoryHT<TypeRepTemplate> AppEnv, TypeRepTemplate def)
		{

			if (String.IsNullOrEmpty(typeRep))
				return def;
			ParseResult<TypeRepTemplate> parseTypes = buildTypeList(typeRep, AppEnv);
			if (parseTypes.Parses != null && parseTypes.Parses.Count == 1 && 
			    String.IsNullOrEmpty(parseTypes.RemainingStr.Trim()))
			{
				return parseTypes.Parses.ToArray()[0] ?? def;
			}
			else
			{
				return def;
			}
		}

		#region deserialization
		
		private static XmlReaderSettings _templateReaderSettings = null;

		/// <summary>
		/// Reader Settings used when reading translation templates.  Validate against schemas   
		/// </summary>
		public static XmlReaderSettings TemplateReaderSettings
		{
			get
			{
				if (_templateReaderSettings == null)
				{
					_templateReaderSettings = new XmlReaderSettings();
					_templateReaderSettings.ValidationType = ValidationType.Schema;
					_templateReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
					_templateReaderSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
				}
				return _templateReaderSettings;
			}
		}

		// Display any warnings or errors while validating translation templates.
		private static void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Warning)
				Console.WriteLine("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
			else
				Console.WriteLine("\tValidation error: " + args.Message);
		}

		private static object Deserialize (Stream fs, System.Type t)
		{
			object o = null;
	
			// Create the XmlReader object.
			// XmlReader reader = XmlReader.Create(fs, TemplateReaderSettings);
		
			XmlSerializer serializer = new XmlSerializer (t, Constants.TranslationTemplateNamespace);
			//o = serializer.Deserialize (reader);
			o = serializer.Deserialize (fs);
			return o;
		}


		private static TypeRepTemplate Deserialize (Stream s)
		{
			TypeRepTemplate ret = null;
			
			XmlReaderSettings settings = new XmlReaderSettings ();
			settings.IgnoreWhitespace = true;
			settings.IgnoreComments = true;
			XmlReader reader = XmlReader.Create (s, settings);
			
			//XmlTextReader reader = new XmlTextReader(s);
			string typeType = null;
			// class, interface, enum, etc.
			bool found = false;

			while (reader.Read () && !found) {
				if (reader.NodeType == XmlNodeType.Element) {
					switch (reader.LocalName) {
						case "Class":
							typeType = "Twiglet.CS2J.Translator.TypeRep.ClassRepTemplate";
							break;
						case "Struct":
							typeType = "Twiglet.CS2J.Translator.TypeRep.StructRepTemplate";
							break;
						case "Interface":
							typeType = "Twiglet.CS2J.Translator.TypeRep.InterfaceRepTemplate";
							break;
						case "Enum":
							typeType = "Twiglet.CS2J.Translator.TypeRep.EnumRepTemplate";
							break;
						case "Delegate":
							typeType = "Twiglet.CS2J.Translator.TypeRep.DelegateRepTemplate";
							break;
						default:
							typeType = "UnknownType";
							break;
					}
					found = true;
				}
			}
			s.Seek (0, SeekOrigin.Begin);
			ret = (TypeRepTemplate)Deserialize (s, System.Type.GetType (typeType));
			
			return ret;
		}


		public static TypeRepTemplate newInstance (Stream s)
		{
			return (TypeRepTemplate)Deserialize (s);
		}
		
		#endregion deserialization
		
		#region Equality
		public bool Equals (TypeRepTemplate other)
		{
			if (other == null)
				return false;
			
			if (Uses != other.Uses) {
				if (Uses == null || other.Uses == null || Uses.Length != other.Uses.Length)
					return false;
				for (int i = 0; i < Uses.Length; i++) {
					if (Uses[i] != other.Uses[i])
						return false;
				}
			}
			if (Aliases != other.Aliases) {
				if (Aliases == null || other.Aliases == null || Aliases.Length != other.Aliases.Length)
					return false;
				for (int i = 0; i < Aliases.Length; i++) {
					if (Aliases[i] != other.Aliases[i])
						return false;
				}
			}
			if (TypeParams != other.TypeParams) {
				if (TypeParams == null || other.TypeParams == null || TypeParams.Length != other.TypeParams.Length)
					return false;
				for (int i = 0; i < TypeParams.Length; i++) {
					if (TypeParams[i] != other.TypeParams[i])
						return false;
				}
			}
			
			if (Casts != other.Casts) {
				if (Casts == null || other.Casts == null || Casts.Count != other.Casts.Count)
					return false;
				for (int i = 0; i < Casts.Count; i++) {
					if (Casts[i] != other.Casts[i])
						return false;
				}
			}
			if (InstantiatedTypes != other.InstantiatedTypes)
			{
				if (InstantiatedTypes == null || other.InstantiatedTypes == null || InstantiatedTypes.Length != other.InstantiatedTypes.Length)
					return false;
				for (int i = 0; i < InstantiatedTypes.Length; i++)
				{
					if (InstantiatedTypes[i] != other.InstantiatedTypes[i])
						return false;
				}
			}

			return IsExplicitNull == other.IsExplicitNull && IsUnboxedType == other.IsUnboxedType && 
			       TypeName == other.TypeName && Variant == other.Variant && BoxedJava == other.BoxedJava && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			TypeRepTemplate temp = obj as TypeRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (TypeRepTemplate a1, TypeRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (TypeRepTemplate a1, TypeRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = base.GetHashCode ();
			if (Uses != null) {
				foreach (string e in Uses) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Aliases != null) {
				foreach (AliasRepTemplate e in Aliases) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (TypeParams != null) {
				foreach (string e in TypeParams) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Casts != null) {
				foreach (CastRepTemplate e in Casts) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (InstantiatedTypes != null)
			{
				foreach (TypeRepTemplate ty in InstantiatedTypes)
				{
					hashCode ^= ty.GetHashCode();
				}
			}

			return (Java ?? String.Empty).GetHashCode() ^ IsExplicitNull.GetHashCode() ^ IsUnboxedType.GetHashCode() ^ 
			       Variant.GetHashCode() ^ BoxedJava.GetHashCode() ^ hashCode;
		}
		#endregion		
		
		public string mkFormattedTypeName(bool incNameSpace, string langle, string rangle)
		{
			StringBuilder fmt = new StringBuilder();
			if (TypeName == "System.Array")
			{
				fmt.Append(InstantiatedTypes[0].mkFormattedTypeName(incNameSpace, langle, rangle));
				fmt.Append("[]");
			}
			else
			{
				fmt.Append(TypeName.Substring(incNameSpace ? 0 : TypeName.LastIndexOf('.')+1));
				if (InstantiatedTypes != null && InstantiatedTypes.Length > 0)
				{
					bool isFirst = true;
					fmt.Append(langle);
					foreach (TypeRepTemplate t in InstantiatedTypes)
					{
						if (!isFirst)
							fmt.Append(", ");
						fmt.Append(t.mkFormattedTypeName(incNameSpace, langle, rangle));
						isFirst = false;
					}
					fmt.Append(rangle);
				}
			}
			return fmt.ToString(); 
		}

		public string mkFormattedTypeName()
		{
			return mkFormattedTypeName(true, "<", ">");
		}

		public string mkSafeTypeName()
		{
			return mkFormattedTypeName(true, "*[", "]*");
		}

		public override String ToString()
		{
			return mkFormattedTypeName();
		}
	}
}