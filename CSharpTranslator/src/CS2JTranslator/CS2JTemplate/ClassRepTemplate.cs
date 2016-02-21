namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[XmlType("Class")]
	public class ClassRepTemplate : InterfaceRepTemplate, IEquatable<ClassRepTemplate>
	{

		private List<ConstructorRepTemplate> _constructors = null;
		[XmlArrayItem("Constructor")]
		public List<ConstructorRepTemplate> Constructors {
			get {
				if (_constructors == null)
					_constructors = new List<ConstructorRepTemplate> ();
				return _constructors;
			}
		}

		private List<FieldRepTemplate> _fields = null;
		[XmlArrayItem("Field")]
		public List<FieldRepTemplate> Fields {
			get {
				if (_fields == null)
					_fields = new List<FieldRepTemplate> ();
				return _fields;
			}
		}

		private List<MethodRepTemplate> _unaryOps = null;
		[XmlArrayItem("UnaryOp")]
		public List<MethodRepTemplate> UnaryOps {
			get {
				if (_unaryOps == null)
					_unaryOps = new List<MethodRepTemplate> ();
				return _unaryOps;
			}
		}

		private List<MethodRepTemplate> _binaryOps = null;
		[XmlArrayItem("BinaryOp")]
		public List<MethodRepTemplate> BinaryOps {
			get {
				if (_binaryOps == null)
					_binaryOps = new List<MethodRepTemplate> ();
				return _binaryOps;
			}
		}

		public ClassRepTemplate ()
		{
		}

		public ClassRepTemplate(ClassRepTemplate copyFrom)
			: base(copyFrom)
		{
			foreach (ConstructorRepTemplate c in copyFrom.Constructors)
			{
				Constructors.Add(new ConstructorRepTemplate(this, c));
			}

			foreach (FieldRepTemplate f in copyFrom.Fields)
			{
				Fields.Add(new FieldRepTemplate(this, f));
			}

			foreach (MethodRepTemplate u in copyFrom.UnaryOps)
			{
				UnaryOps.Add(new MethodRepTemplate(this, u));
			}

			foreach (MethodRepTemplate b in copyFrom.BinaryOps)
			{
				BinaryOps.Add(new MethodRepTemplate(this, b));
			}

		}

		public ClassRepTemplate(string typeName)
			: base(typeName)
		{
		}

		public ClassRepTemplate(string tName, string[] tParams, string[] usePath, AliasRepTemplate[] aliases, string[] inherits, List<ConstructorRepTemplate> cs, List<MethodRepTemplate> ms, List<PropRepTemplate> ps, List<FieldRepTemplate> fs, List<FieldRepTemplate> es, List<IndexerRepTemplate> ixs, List<CastRepTemplate> cts,
			string[] imports, string javaTemplate) 
			: base(tName, tParams, usePath, aliases, inherits, ms, ps, es, ixs, imports, javaTemplate)
		{
			_constructors = cs;
			_fields = fs;
			_casts = cts;
		}

		public ClassRepTemplate (string tName, string[] tParams, string[] usePath, AliasRepTemplate[] aliases, string[] inherits, List<ConstructorRepTemplate> cs, List<MethodRepTemplate> ms, List<PropRepTemplate> ps, List<FieldRepTemplate> fs, List<FieldRepTemplate> es, List<IndexerRepTemplate> ixs, List<CastRepTemplate> cts)
			: base(tName, tParams, usePath, aliases, inherits, ms, ps, es, ixs, null, null)
		{
			_constructors = cs;
			_fields = fs;
			_casts = cts;
		}

		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (Constructors != null)
			{
				foreach(ConstructorRepTemplate c in Constructors)
				{
					c.Apply(args);
				}
			}
			if (Fields != null)
			{
				foreach(FieldRepTemplate f in Fields)
				{
					f.Apply(args);
				}
			}

			if (UnaryOps != null)
			{
				foreach(MethodRepTemplate u in UnaryOps)
				{
					u.Apply(args);
				}
			}

			if (BinaryOps != null)
			{
				foreach(MethodRepTemplate b in BinaryOps)
				{
					b.Apply(args);
				}
			}
			base.Apply(args);
		}


		public override ResolveResult Resolve(String name, IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
        
			// Look for a property which holds a delegate with the right type
			if (Fields != null)
			{
				foreach (FieldRepTemplate f in Fields)
				{
					if (f.Name == name)
					{
						// Is f's type a delegate?
						DelegateRepTemplate del = BuildType(f.Type, AppEnv, null) as DelegateRepTemplate;
						if (del != null && matchParamsToArgs(del.Invoke.Params, del.Invoke.ParamArray, args, AppEnv, implicitCast))
						{
							ResolveResult delRes = new ResolveResult();
							delRes.Result = del;
							delRes.ResultType = BuildType(del.Invoke.Return, AppEnv);
							DelegateResolveResult res = new DelegateResolveResult();
							res.Result = f;
							res.ResultType = BuildType(f.Type, AppEnv);
							res.DelegateResult = delRes;
							return res;
						}
					}
				}            
			}
			return base.Resolve(name, args, AppEnv, implicitCast);
		}

		public override ResolveResult Resolve(String name, bool forWrite, DirectoryHT<TypeRepTemplate> AppEnv,bool implicitCast)
		{
        
			if (Fields != null)
			{
				foreach (FieldRepTemplate f in Fields)
				{
					if (f.Name == name)
					{
						ResolveResult res = new ResolveResult();
						res.Result = f;
						res.ResultType = BuildType(f.Type, AppEnv);
						return res;
					}
				}
			}
			return base.Resolve(name, forWrite, AppEnv,implicitCast);
		}

		public ResolveResult Resolve(IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv)
		{
			ResolveResult res = Resolve(args, AppEnv, false);
			if (TemplateUtilities.DO_IMPLICIT_CASTS && res == null) res = Resolve(args, AppEnv, true);
			return res;
		}
		public ResolveResult Resolve(IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
        
			if (Constructors != null)
			{
				foreach (ConstructorRepTemplate c in Constructors)
				{
					if (matchParamsToArgs(c.Params, c.ParamArray, args, AppEnv, implicitCast))
					{
						ResolveResult res = new ResolveResult();
						res.Result = c;
						res.ResultType = this;
						return res;
					}
				}
			}
			// We don't search base,  constructors aren't inherited
			return null;
		}
		public override TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args)
		{
			ClassRepTemplate copy = new ClassRepTemplate(this);
			if (args != null && args.Count != 0) {
				copy.Apply(mkTypeMap(args));
			}
			return copy;
		}

		#region Equality
		public bool Equals (ClassRepTemplate other)
		{
			if (other == null)
				return false;
			
			if (Constructors != other.Constructors) {
				if (Constructors == null || other.Constructors == null || Constructors.Count != other.Constructors.Count)
					return false;
				for (int i = 0; i < Constructors.Count; i++) {
					if (Constructors[i] != other.Constructors[i])
						return false;
				}
			}

			if (Fields != other.Fields) {
				if (Fields == null || other.Fields == null || Fields.Count != other.Fields.Count)
					return false;
				for (int i = 0; i < Fields.Count; i++) {
					if (Fields[i] != other.Fields[i])
						return false;
				}
			}

			if (UnaryOps != other.UnaryOps) {
				if (UnaryOps == null || other.UnaryOps == null || UnaryOps.Count != other.UnaryOps.Count)
					return false;
				for (int i = 0; i < UnaryOps.Count; i++) {
					if (UnaryOps[i] != other.UnaryOps[i])
						return false;
				}
			}

			if (BinaryOps != other.BinaryOps) {
				if (BinaryOps == null || other.BinaryOps == null || BinaryOps.Count != other.BinaryOps.Count)
					return false;
				for (int i = 0; i < BinaryOps.Count; i++) {
					if (BinaryOps[i] != other.BinaryOps[i])
						return false;
				}
			}


			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			ClassRepTemplate temp = obj as ClassRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (ClassRepTemplate a1, ClassRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (ClassRepTemplate a1, ClassRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = base.GetHashCode ();
			if (Constructors != null) {
				foreach (ConstructorRepTemplate e in Constructors) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Fields != null) {
				foreach (FieldRepTemplate e in Fields) {
					hashCode ^= e.GetHashCode();
				}
			}

			return hashCode;
		}
		#endregion	
         
	}
}