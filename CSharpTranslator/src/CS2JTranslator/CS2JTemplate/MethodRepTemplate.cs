namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Serialization;

	public class MethodRepTemplate : ConstructorRepTemplate, IEquatable<MethodRepTemplate>
	{
		// Method name
		public string Name { get; set; }

		// Method name in Java (defaults to Name)
		private string _javaName = null;
		public string JavaName { 
			get
			{
				return (_javaName == null || _javaName.Length == 0) ? Name : _javaName; 
			}
			set
			{
				_javaName = value;
			}
		}

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

		// Return type
		private TypeRepRef _return = new TypeRepRef();
		public TypeRepRef Return { 
			get { return _return; }
			set {
				_return=value;
			}
		}		

		// isStatic method?
		private bool _isStatic = false;
		[XmlAttribute("static")]
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool IsStatic { 
			get 
			{
				return _isStatic;
			}
			set
			{
				_isStatic = value;
			}
		}

		private bool _isPartialDefiner = false;
		[XmlAttribute("partial")]
		[System.ComponentModel.DefaultValueAttribute(false)]
		public bool IsPartialDefiner { 
			get 
			{
				return _isPartialDefiner;
			}
			set
			{
				_isPartialDefiner = value;
			}
		}

		public MethodRepTemplate()
		{
			IsStatic = false;
			IsPartialDefiner = false;
		}

		public MethodRepTemplate(TypeRepTemplate parent, MethodRepTemplate copyFrom) : base(parent, copyFrom)
		{
			if (!String.IsNullOrEmpty(copyFrom.Name))
			{
				Name = copyFrom.Name;
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
					InstantiatedTypes[i] = copyFrom.InstantiatedTypes[i].Instantiate(null);
				}
			}
			Return = new TypeRepRef(copyFrom.Return);

			IsStatic = copyFrom.IsStatic;
			IsPartialDefiner = copyFrom.IsPartialDefiner;
		}

		public MethodRepTemplate(string retType, string methodName, string[] tParams, List<ParamRepTemplate> pars, string[] imps, string javaRep)
			: base(pars, imps, javaRep)
		{
			Name = methodName;
			TypeParams = tParams;
			Return = new TypeRepRef(retType);
			IsStatic = false;
			IsPartialDefiner = false;
		}

		public MethodRepTemplate (string retType, string methodName, string[] tParams, List<ParamRepTemplate> pars) : this(retType, methodName, tParams, pars, null, null)
		{
		}
		
		public override string[] mkImports() {
			if (IsStatic && SurroundingType != null) {
				return new string[] {SurroundingType.TypeName};
			}
			else {
				return null;
			}
		}	
		
		public override string mkJava() {
			StringBuilder methStr = new StringBuilder();

			// if we only have the definition, not the implementation, then don't emit any calls in the Java
			if (IsPartialDefiner)
			{
				return String.Empty;
			}

			if (IsStatic) {
				if (SurroundingType != null) {
					methStr.Append(SurroundingType.TypeName.Substring(SurroundingType.TypeName.LastIndexOf('.') + 1) + ".");
				}
				else {
					methStr.Append("TYPENAME.");
				}
			}
			else {
				methStr.Append("${this:16}.");
			}

			methStr.Append(JavaName);
         
			return methStr.ToString() + mkJavaParams(Params, ParamArray);
		}
		
		// TODO: filter out redefined type names
		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (Return != null)
			{
				Return.SubstituteInType(args);
			}
			base.Apply(args);
		}

		#region Equality
		public bool Equals (MethodRepTemplate other)
		{
			if (other == null)
				return false;

			if (TypeParams != other.TypeParams) {
				if (TypeParams == null || other.TypeParams == null || TypeParams.Length != other.TypeParams.Length)
					return false;
				for (int i = 0; i < TypeParams.Length; i++) {
					if (TypeParams[i] != other.TypeParams[i])
						return false;
				}
			}
			if (InstantiatedTypes != other.InstantiatedTypes) {
				if (InstantiatedTypes == null || other.InstantiatedTypes == null || InstantiatedTypes.Length != other.InstantiatedTypes.Length)
					return false;
				for (int i = 0; i < InstantiatedTypes.Length; i++) {
					if (InstantiatedTypes[i] != other.InstantiatedTypes[i])
						return false;
				}
			}
			
			return Return == other.Return && Name == other.Name && JavaName == other.JavaName && IsStatic == other.IsStatic && IsPartialDefiner == other.IsPartialDefiner && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			MethodRepTemplate temp = obj as MethodRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (MethodRepTemplate a1, MethodRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (MethodRepTemplate a1, MethodRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = 0;
			if (TypeParams != null) {
				foreach (string o in TypeParams) {
					hashCode = hashCode ^ o.GetHashCode() ;
				}
			}
			if (InstantiatedTypes != null) {
				foreach (TypeRepTemplate o in InstantiatedTypes) {
					hashCode = hashCode ^ o.GetHashCode() ;
				}
			}

			hashCode = hashCode ^ (Return != null ? Return.GetHashCode() : 0);


			return hashCode ^ (Name ?? String.Empty).GetHashCode () ^ (JavaName ?? String.Empty).GetHashCode () ^ IsStatic.GetHashCode() ^ IsPartialDefiner.GetHashCode() ^ base.GetHashCode();
		}
		#endregion

	}
}