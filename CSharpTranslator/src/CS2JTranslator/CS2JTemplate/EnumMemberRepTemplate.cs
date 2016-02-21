namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;

	public class EnumMemberRepTemplate : TranslationBase, IEquatable<EnumMemberRepTemplate>
	{

		public string Name { get; set; }
		public string Value { get; set; }


		public EnumMemberRepTemplate() : base()
		{
		}
		public EnumMemberRepTemplate(TypeRepTemplate parent, EnumMemberRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			if (!String.IsNullOrEmpty(copyFrom.Name))
			{
				Name = copyFrom.Name;
			}

			if (!String.IsNullOrEmpty(copyFrom.Value))
			{
				Value = copyFrom.Value;
			}
		}

		public EnumMemberRepTemplate (string n) : this(n, null, null, null)
		{
		}

		public EnumMemberRepTemplate (string n, string v) : this(n, v, null, null)
		{
		}

		public EnumMemberRepTemplate (string n, string v, string[] imps, string java) : base(imps, java)
		{
			Name = n;
			Value = v;
		}		
		
		public override string mkJava() {
			return "${this:16}." + Name;
		}


		#region Equality
		public bool Equals (EnumMemberRepTemplate other)
		{
			if (other == null)
				return false;
			
			return Name == other.Name && Value == other.Value && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			EnumMemberRepTemplate temp = obj as EnumMemberRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (EnumMemberRepTemplate a1, EnumMemberRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (EnumMemberRepTemplate a1, EnumMemberRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			return (Name ?? String.Empty).GetHashCode () ^ (Value ?? String.Empty).GetHashCode () ^ base.GetHashCode ();
		}
		#endregion		
		
	}
}