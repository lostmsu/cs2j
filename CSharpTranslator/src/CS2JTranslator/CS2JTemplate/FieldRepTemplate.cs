namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;

	public class FieldRepTemplate : TranslationBase, IEquatable<FieldRepTemplate>
	{

		private TypeRepRef _type;
		public TypeRepRef Type { 
			get { return _type; }
			set {
				_type=value;
			}
		}		
		public string Name { get; set; }

		public FieldRepTemplate()
			: base()
		{
		}

		public FieldRepTemplate(TypeRepTemplate parent, FieldRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			if (!String.IsNullOrEmpty(copyFrom.Name))
			{
				Name = copyFrom.Name;
			}
			if (copyFrom.Type != null)
			{
				Type = new TypeRepRef(copyFrom.Type);
			}
		}

		public FieldRepTemplate(string fType, string fName, string[] imps, string javaGet)
			: base(imps, javaGet)
		{
			Type = new TypeRepRef(fType);
			Name = fName;
		}

		public FieldRepTemplate (string fType, string fName) : this(fType, fName, null, null)
		{
		}
		
				
		public override string mkJava() {
			return "${this:16}." + Name;
		}

		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (Type != null)
			{
				Type.SubstituteInType(args);
			}
			base.Apply(args);
		}

		#region Equality
		public bool Equals (FieldRepTemplate other)
		{
			if (other == null)
				return false;
			
			return Type == other.Type && Name == other.Name && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			FieldRepTemplate temp = obj as FieldRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (FieldRepTemplate a1, FieldRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (FieldRepTemplate a1, FieldRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = Type != null ? Type.GetHashCode() : 0;
			return hashCode ^ (Name ?? String.Empty).GetHashCode() ^ base.GetHashCode();
		}
		#endregion
		
	}
}