namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Serialization;

	public abstract class TranslationBase : IEquatable<TranslationBase>, IApplyTypeArgs
	{
		// Java imports required to make Java translation run
		private string[] _imports = null;
		[XmlArrayItem("Import")]
		public virtual string[] Imports { 
			get {
				// if _java is not set then see if we have default imports, otherwise
				// assume imports is already correctly (un)set
				if (_imports == null && _java == null) {
					return mkImports();
				}
				return _imports;
			}
			set { _imports = value; } 
		}
		
		// The Java translation for this C# entity
		protected string _java = null; 		
		public virtual string Java { 
			get { 
				if (_java == null) {
					return mkJava();
				} 
				else {
					return _java;
				}
			}
			set { _java = value; } 
		}
		
		// Emit this warning if we use this translation
		protected string _warning = null; 		
		public virtual string Warning { 
			get { return _warning; }
			set { _warning = value; } 
		}
		
		// Optional,  but if present will let mkJava generate better java guess in some cases
		private TypeRepTemplate _surroundingType;
		[XmlIgnore]
		public TypeRepTemplate SurroundingType { 
			get { return _surroundingType; }
			set {
				_surroundingType=value;
			}
		}		
		public virtual string[] mkImports() {
			return null;
		}
		
		public string[] mkImports(Javastyle style) {
			string[] imports = mkImports();
			if (style == Javastyle.MarkAuto) {
				for (int i = 0; i < imports.Length; i++) {
					imports[i] = imports[i] + " /*auto*/";
				}
			}
			return imports;
		}

		public abstract string mkJava(); 
		
		public string mkJava(Javastyle style) {
			string unAdornedJava = mkJava();
			if (style == Javastyle.MarkAuto) {
				return "/*auto (/*" + unAdornedJava + "/*)*/";
			}
			else {
				return unAdornedJava;
			}
		}

		protected TranslationBase()
		{
			Imports = null;
		}

		protected TranslationBase(TypeRepTemplate parent, TranslationBase copyFrom)
		{
			int len = 0;
			if (copyFrom.Imports != null)
			{
				len = copyFrom.Imports.Length;
				Imports = new String[len];
				for (int i = 0; i < len; i++)
				{
					Imports[i] = copyFrom.Imports[i];
				}
			}
			if (!String.IsNullOrEmpty(copyFrom.Java))
			{
				Java = copyFrom.Java;
			}

			if (!String.IsNullOrEmpty(copyFrom.Warning))
			{
				Warning = copyFrom.Warning;
			}

			SurroundingType = parent;
		}

		protected TranslationBase(string java)
		{
			Imports = null;
			Java = java;
		}

		protected TranslationBase (string[] imps, string java)
		{
			Imports = imps;
			Java = java;
		}


		protected string mkJavaParams(IList<ParamRepTemplate> pars, ParamArrayRepTemplate paramarr) {
			StringBuilder parStr = new StringBuilder();
			parStr.Append("(");
			foreach (ParamRepTemplate p in pars) {
				parStr.Append("${"+p.Name+"},");
			}
			if (parStr[parStr.Length-1] == ',') {
				// remove trailing comma
				parStr.Remove(parStr.Length-1,1);
			}
			if (paramarr != null)
			{
				// ${*]n} means all parameters with position (1,2,..) greater than n
				// so ${*]0} means all arguments, ${*]1} means all but first etc.
				parStr.Append("${" + (pars.Count > 0 ? "," : "") + "*]"+pars.Count.ToString()+"}");
			}
			parStr.Append(")");
			return parStr.ToString();
		}
		protected string mkTypeParams(String[] pars) {
			StringBuilder parStr = new StringBuilder();
			parStr.Append("*[");
			foreach (string p in pars) {
				parStr.Append("${"+p+"},");
			}
			if (parStr[parStr.Length-1] == ',') {
				parStr.Remove(parStr.Length-1,1);
			}
			parStr.Append("]*");
			return parStr.ToString();
		}

		// Instantiate type arguments
		public virtual void Apply(Dictionary<string,TypeRepTemplate> args)
		{
		}

		#region Equality

		public bool Equals (TranslationBase other)
		{
			if (other == null)
				return false;
			
			if (Imports != other.Imports) {
				if (Imports == null || other.Imports == null || Imports.Length != other.Imports.Length)
					return false;
				for (int i = 0; i < Imports.Length; i++) {
					if (Imports[i] != other.Imports[i])
						return false;
				}
			}
			
			return Java == other.Java && Warning == other.Warning;
		}

		public override bool Equals (object obj)
		{
			
			TranslationBase temp = obj as TranslationBase;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (TranslationBase a1, TranslationBase a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (TranslationBase a1, TranslationBase a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = 0;
			if (Imports != null) {
				foreach (string e in Imports) {
					hashCode ^= e.GetHashCode();
				}
			}
			return (Java ?? String.Empty).GetHashCode () ^ (Warning ?? String.Empty).GetHashCode () ^ hashCode;
		}
		#endregion
		
	}
}