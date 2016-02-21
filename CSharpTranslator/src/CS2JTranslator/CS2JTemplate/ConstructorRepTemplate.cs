namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class ConstructorRepTemplate : TranslationBase, IEquatable<ConstructorRepTemplate>
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
			get
			{
				return _paramArray;
			}
			set
			{
				_paramArray = value;
			}

		}
		
		public override string mkJava() {
			string constructorName = "CONSTRUCTOR";
			if (SurroundingType != null) {
				constructorName = SurroundingType.TypeName.Substring(SurroundingType.TypeName.LastIndexOf('.') + 1);
				if (SurroundingType.TypeParams != null && SurroundingType.TypeParams.Length > 0)
				{
					constructorName += mkTypeParams(SurroundingType.TypeParams);
				}
			}
			return "new " + constructorName + mkJavaParams(Params, ParamArray);
		}
		
		public override string[] mkImports() {
			if (SurroundingType != null) {
				return new string[] {SurroundingType.TypeName};
			}
			else {
				return null;
			}
		}

		public ConstructorRepTemplate()
			: base()
		{
		}

		public ConstructorRepTemplate(TypeRepTemplate parent, ConstructorRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			foreach (ParamRepTemplate p in copyFrom.Params)
			{
				Params.Add(new ParamRepTemplate(p));
			}
			ParamArray = copyFrom.ParamArray;
		}

		public ConstructorRepTemplate (List<ParamRepTemplate> pars) : base()
		{
			_params = pars;
		}

		public ConstructorRepTemplate (List<ParamRepTemplate> pars, string[] imps, string javaRep) : base(imps, javaRep)
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
			if (ParamArray != null)
				ParamArray.Apply(args);
         
			base.Apply(args);
		}

		#region Equality

		public bool Equals (ConstructorRepTemplate other)
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

			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			ConstructorRepTemplate temp = obj as ConstructorRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (ConstructorRepTemplate a1, ConstructorRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (ConstructorRepTemplate a1, ConstructorRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = 0;
			foreach (ParamRepTemplate o in Params) {
				hashCode = hashCode ^ o.GetHashCode() ;
			}
			
			return base.GetHashCode () ^ hashCode ^ (ParamArray == null ? 0 : ParamArray.GetHashCode());
		}
		#endregion
	}
}