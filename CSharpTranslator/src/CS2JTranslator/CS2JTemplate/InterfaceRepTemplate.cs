namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[XmlType("Interface")]
	public class InterfaceRepTemplate : TypeRepTemplate, IEquatable<InterfaceRepTemplate>
	{
		private List<MethodRepTemplate> _methods = null;
		[XmlArrayItem("Method")]
		public List<MethodRepTemplate> Methods {
			get {
				if (_methods == null)
					_methods = new List<MethodRepTemplate> ();
				return _methods;
			}
		}
		
		private List<PropRepTemplate> _properties = null;
		[XmlArrayItem("Property")]
		public List<PropRepTemplate> Properties {
			get {
				if (_properties == null)
					_properties = new List<PropRepTemplate> ();
				return _properties;
			}
		}
		
		private List<FieldRepTemplate> _events = null;
		[XmlArrayItem("Event")]
		public List<FieldRepTemplate> Events {
			get {
				if (_events == null)
					_events = new List<FieldRepTemplate> ();
				return _events;
			}
		}
		
		private List<IndexerRepTemplate> _indexers = null;
		[XmlArrayItem("Indexer")]
		public List<IndexerRepTemplate> Indexers {
			get {
				if (_indexers == null)
					_indexers = new List<IndexerRepTemplate> ();
				return _indexers;
			}
		}
		
		private IterableRepTemplate _iterable = null;
		public IterableRepTemplate Iterable {
			get {
				return _iterable;
			}
			set {
				_iterable = value;
			}
		}
		
		public InterfaceRepTemplate () : base()
		{
			Inherits = null;
		}

		public InterfaceRepTemplate(InterfaceRepTemplate copyFrom)
			: base(copyFrom)
		{
			foreach (MethodRepTemplate m in copyFrom.Methods)
			{
				Methods.Add(new MethodRepTemplate(this, m));
			}

			foreach (PropRepTemplate p in copyFrom.Properties)
			{
				Properties.Add(new PropRepTemplate(this, p));
			}

			foreach (FieldRepTemplate e in copyFrom.Events)
			{
				Events.Add(new FieldRepTemplate(this, e));
			}

			foreach (IndexerRepTemplate i in copyFrom.Indexers)
			{
				Indexers.Add(new IndexerRepTemplate(this, i));
			}

			if (copyFrom.Iterable != null)
			{
				Iterable = new IterableRepTemplate(this, copyFrom.Iterable);
			}
		}

		public InterfaceRepTemplate(string typeName)
			: base(typeName)
		{
		}

		protected InterfaceRepTemplate(string tName, string[] tParams, string[] usePath, AliasRepTemplate[] aliases, string[] inherits, List<MethodRepTemplate> ms, List<PropRepTemplate> ps, List<FieldRepTemplate> es, List<IndexerRepTemplate> ixs, string[] imps, string javaTemplate) 
			: base(tName, tParams, usePath, aliases, imps, javaTemplate)
		{
			Inherits = inherits;
			_methods = ms;
			_properties = ps;
			_events = es;
			_indexers = ixs;
		}

		
		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (Methods != null)
			{
				foreach(MethodRepTemplate m in Methods)
				{
					m.Apply(args);
				}
			}
			if (Properties != null)
			{
				foreach(PropRepTemplate p in Properties)
				{
					p.Apply(args);
				}
			}
			if (Events != null)
			{
				foreach(FieldRepTemplate e in Events)
				{
					e.Apply(args);
				}
			}
			if (Indexers != null)
			{
				foreach (IndexerRepTemplate i in Indexers)
				{
					i.Apply(args);
				}
			}
			if (Iterable != null)
			{
				Iterable.Apply(args);
			}
			base.Apply(args);
		}

		// Returns true if we are a subclass of other, or implements its interface
//       public override bool IsA (TypeRepTemplate other,  DirectoryHT<TypeRepTemplate> AppEnv) {
//          InterfaceRepTemplate i = other as InterfaceRepTemplate;
//          if (i == null)
//          {
//             return false;                         
//          }
//          if (i.TypeName == this.TypeName)
//          {
//             return true;
//          }
//          return base.IsA(other,AppEnv);
//       }
// 
		public override ResolveResult Resolve(String name, bool forWrite, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
        
			if (Properties != null)
			{
				foreach (PropRepTemplate p in Properties)
				{
					if (p.Name == name && ((forWrite && p.CanWrite) || (!forWrite && p.CanRead)))
					{
						ResolveResult res = new ResolveResult();
						res.Result = p;
						res.ResultType = BuildType(p.Type, AppEnv);
						return res;
					}
				}
			}
			return base.Resolve(name, forWrite, AppEnv, implicitCast);
		}

		/// <summary>
		/// Can we match Params + ParamArray against args   
		/// </summary>
		/// 
		protected bool matchParamsToArgs(IList<ParamRepTemplate> param, ParamArrayRepTemplate paramArray, IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			int argsLength = args == null ? 0 : args.Count;
			int paramsLength = param == null ? 0 : param.Count;
         
			if (paramsLength > 0)
			{
				// Check fixed parameters against args 
				if (argsLength < paramsLength)
				{
					// Length of required fixed Parameters is greater than number of available arguments 
					return false; 
				}
				else
				{
					// Check fixed Parameters against args
					// check that for each argument in the caller its type 'IsA' the type of the formal parameter
					for (int idx = 0; idx < paramsLength; idx++) {
						if (args[idx] == null || !args[idx].IsA(BuildType(param[idx].Type, AppEnv, new UnknownRepTemplate(param[idx].Type.Type)), AppEnv, implicitCast))
						{
							// An argument doesn't match
							return false; 
						}
					}
				}
			}

			if (argsLength == paramsLength)
				// OK, fixed args check out.
				return true;

			if (paramArray == null)
				// Extra args and no param array
				return false;

			// We have args left over, check param argument. 

			String paramsTypeStr = paramArray.Type.Type ?? "System.Object";
			if (!paramsTypeStr.EndsWith("[]"))
				// Type should be an array, maybe print a warning if it isn't?
				paramsTypeStr = paramsTypeStr + "[]";

			TypeRepTemplate paramsType = BuildType(paramsTypeStr, AppEnv, new UnknownRepTemplate(paramsTypeStr));

			if (argsLength == paramsLength + 1)
			{
				if (args[argsLength - 1].IsA(paramsType, AppEnv, implicitCast))
				{
					// Can pass an array as final argument
					return true;
				}
			}
  
			// Check additional args against params element type
			// Remove final array marker
			paramsTypeStr = paramsTypeStr.Remove(paramsTypeStr.Length-2);
			paramsType = BuildType(paramsTypeStr, AppEnv, new UnknownRepTemplate(paramsTypeStr));

			for (int idx = paramsLength; idx < argsLength; idx++)
			{
				if (args[idx] == null || !args[idx].IsA(paramsType,AppEnv,implicitCast))
					return false;
			}
			return true;
		}

		public override ResolveResult Resolve(String name, IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			if (Methods != null)
			{
				ResolveResult res = null;
				foreach (MethodRepTemplate m in Methods)
				{
					if (m.Name == name && matchParamsToArgs(m.Params, m.ParamArray, args, AppEnv, implicitCast))
					{
						res = new ResolveResult();
						res.Result = m;
						res.ResultType = BuildType(m.Return, AppEnv);
						if (!m.IsPartialDefiner)
							return res;
					}
				}
				if (res != null)
				{
					// We must have only found a partial result, nothing to implement it, so return the partial result 
					return res;
				}

			}
			// Look for a property which holds a delegate with the right type
			if (Properties != null)
			{
				foreach (PropRepTemplate p in Properties)
				{
					if (p.Name == name)
					{
						// Is p's type a delegate?
						DelegateRepTemplate del = BuildType(p.Type, AppEnv, null) as DelegateRepTemplate;
						if (del != null && matchParamsToArgs(del.Invoke.Params, del.Invoke.ParamArray, args, AppEnv,implicitCast))
						{
							ResolveResult delRes = new ResolveResult();
							delRes.Result = del;
							delRes.ResultType = BuildType(del.Invoke.Return, AppEnv);
							DelegateResolveResult res = new DelegateResolveResult();
							res.Result = p;
							res.ResultType = BuildType(p.Type, AppEnv);
							res.DelegateResult = delRes;
							return res;                     
						}
					}
				}            
			}
			return base.Resolve(name, args, AppEnv,implicitCast);
		}

		public override ResolveResult ResolveIndexer(IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
        
			if (Indexers != null)
			{
				foreach (IndexerRepTemplate i in Indexers)
				{
					if (matchParamsToArgs(i.Params, i.ParamArray, args, AppEnv, implicitCast))
					{
						ResolveResult res = new ResolveResult();
						res.Result = i;
						res.ResultType = BuildType(i.Type, AppEnv);
						return res;
					}
				}
			}
			return base.ResolveIndexer(args, AppEnv,implicitCast);
		}

		public override ResolveResult ResolveIterable(DirectoryHT<TypeRepTemplate> AppEnv)
		{
        
			if (Iterable != null)
			{
				ResolveResult res = new ResolveResult();
				res.Result = Iterable;
				res.ResultType = BuildType(Iterable.ElementType, AppEnv);
				return res;
			}
			return base.ResolveIterable(AppEnv);
		}
		public override TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args)
		{
			InterfaceRepTemplate copy = new InterfaceRepTemplate(this);
			if (args != null && args.Count != 0) {
				copy.Apply(mkTypeMap(args));
			}
			return copy;
		}

		#region Equality
		public bool Equals (InterfaceRepTemplate other)
		{
			if (other == null)
				return false;
			
			if (Inherits != other.Inherits) {
				if (Inherits == null || other.Inherits == null || Inherits.Length != other.Inherits.Length)
					return false;
				for (int i = 0; i < Inherits.Length; i++) {
					if (Inherits[i] != other.Inherits[i])
						return false;
				}
			}
			
			if (Methods != other.Methods) {
				if (Methods == null || other.Methods == null || Methods.Count != other.Methods.Count)
					return false;
				for (int i = 0; i < Methods.Count; i++) {
					if (Methods[i] != other.Methods[i])
						return false;
				}
			}

			if (Properties != other.Properties) {
				if (Properties == null || other.Properties == null || Properties.Count != other.Properties.Count)
					return false;
				for (int i = 0; i < Properties.Count; i++) {
					if (Properties[i] != other.Properties[i])
						return false;
				}
			}

			if (Events != other.Events) {
				if (Events == null || other.Events == null || Events.Count != other.Events.Count)
					return false;
				for (int i = 0; i < Events.Count; i++) {
					if (Events[i] != other.Events[i])
						return false;
				}
			}

			if (Indexers != other.Indexers) {
				if (Indexers == null || other.Indexers == null || Indexers.Count != other.Indexers.Count)
					return false;
				for (int i = 0; i < Indexers.Count; i++) {
					if (Indexers[i] != other.Indexers[i])
						return false;
				}
			}
			
			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			InterfaceRepTemplate temp = obj as InterfaceRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (InterfaceRepTemplate a1, InterfaceRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (InterfaceRepTemplate a1, InterfaceRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = base.GetHashCode ();
			if (Inherits != null) {
				foreach (string e in Inherits) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Methods != null) {
				foreach (MethodRepTemplate e in Methods) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Properties != null) {
				foreach (PropRepTemplate e in Properties) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Events != null) {
				foreach (FieldRepTemplate e in Events) {
					hashCode ^= e.GetHashCode();
				}
			}
			if (Indexers != null) {
				foreach (IndexerRepTemplate e in Indexers) {
					hashCode ^= e.GetHashCode();
				}
			}
			return hashCode;
		}
		#endregion	
	}
}