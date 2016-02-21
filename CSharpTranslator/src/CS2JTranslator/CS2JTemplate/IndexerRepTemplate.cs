namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class IndexerRepTemplate : PropRepTemplate, IEquatable<IndexerRepTemplate>
	{
		private List<ParamRepTemplate> _params = null;
		[XmlArrayItem("Param")]
		public List<ParamRepTemplate> Params {
			get {
				if (_params == null)
					_params = new List<ParamRepTemplate> ();
				return _params;
			}
		}
		
		private ParamArrayRepTemplate _paramArray = null;
		public ParamArrayRepTemplate ParamArray {
			get {
				return _paramArray;
			}
			set {
				_paramArray = value;
			}
		}
		
		private List<ParamRepTemplate> _setParams = null;
		private List<ParamRepTemplate> SetParams {
			get {
				if (_setParams == null)
				{
					_setParams = new List<ParamRepTemplate> ();
					foreach (ParamRepTemplate p in Params)
					{
						_setParams.Add(p);
					}
					_setParams.Add(new ParamRepTemplate(Type.Type,"value"));
				}
				return _setParams;
			}
		}
		
		private ParamArrayRepTemplate _setParamArray = null;
		public ParamArrayRepTemplate SetParamArray {
			get {
				if (_setParamArray == null)
				{
					return ParamArray;
				}
				else
				{
					return _setParamArray;
				}
			}
			set {
				_setParamArray = value;
			}
		}
		
		[XmlElement("Get")]
		public override string JavaGet {
			get {
				if (!CanRead) return null;
				if (_javaGet == null) {
					if (_java == null) {
						return (CanRead ? "${this:16}.get___idx" + mkJavaParams(Params, ParamArray) : null);
					}
					else {
						return _java;
					}
				}
				else {
					return _javaGet;
				}
			}
			set { _javaGet = value; }
		}
		
		[XmlElement("Set")]
		public override string JavaSet { 
			get {
				if (_javaSet == null) {
					return (CanWrite ? "${this:16}.set___idx" + mkJavaParams(SetParams, SetParamArray): null);
				}
				else {
					return _javaSet;
				}
			}
			set { _javaSet = value; }
		}

		public IndexerRepTemplate()
			: base()
		{
		}

		public IndexerRepTemplate(TypeRepTemplate parent, IndexerRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			foreach (ParamRepTemplate p in copyFrom.Params)
			{
				Params.Add(new ParamRepTemplate(p));
			}
			if (copyFrom._setParams != null)
			{
				foreach (ParamRepTemplate p in copyFrom._setParams)
				{
					SetParams.Add(new ParamRepTemplate(p));
				}
			}

			_paramArray = copyFrom._paramArray;
			_setParamArray = copyFrom._setParamArray;

			if (!String.IsNullOrEmpty(copyFrom.JavaGet))
			{
				JavaGet = copyFrom.JavaGet;
			}
			if (!String.IsNullOrEmpty(copyFrom.JavaSet))
			{
				JavaSet = copyFrom.JavaSet;
			}
		}

		public IndexerRepTemplate(string fType, List<ParamRepTemplate> pars)
			: base(fType, "this")
		{
			_params = pars;
		}

		public IndexerRepTemplate (string fType, List<ParamRepTemplate> pars, string[] imps, string javaGet, string javaSet) : base(fType, "this",imps,javaGet,javaSet)
		{
			_params = pars;
		}


		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (Params != null)
			{
				foreach(ParamRepTemplate p in Params)
				{
					p.Apply(args);
				}
			}
			if (_setParams != null)
			{
				foreach(ParamRepTemplate p in _setParams)
				{
					p.Apply(args);
				}
			}

			if (_paramArray != null)
				_paramArray.Apply(args);
			if (_setParamArray != null)
				_setParamArray.Apply(args);

			base.Apply(args);
		}

		#region Equality

		public bool Equals (IndexerRepTemplate other)
		{
			if (other == null)
				return false;
			
			if (Params != other.Params) {
				if (Params == null || other.Params == null || Params.Count != other.Params.Count)
					return false;
				for (int i = 0; i < Params.Count; i++) {
					if (Params[i] != other.Params[i])
						return false;
				}
			}

			if (ParamArray != other.ParamArray)
				return false;

			if (SetParamArray != other.SetParamArray)
				return false;

			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			IndexerRepTemplate temp = obj as IndexerRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (IndexerRepTemplate a1, IndexerRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (IndexerRepTemplate a1, IndexerRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = 0;
			foreach (ParamRepTemplate o in Params) {
				hashCode = hashCode ^ o.GetHashCode() ;
			}
			
			return base.GetHashCode () ^ hashCode ^  (ParamArray == null ? 0 : ParamArray.GetHashCode()) ^ (_setParamArray == null ? 0 : _setParamArray.GetHashCode());
		}
		#endregion
	}
}