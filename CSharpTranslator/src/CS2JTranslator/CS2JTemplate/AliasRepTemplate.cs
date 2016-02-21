namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;

	public class AliasRepTemplate : IEquatable<AliasRepTemplate>
	{

		public string Alias { get; set; }
		public string Namespace { get; set; }


		public AliasRepTemplate()
		{
			Alias = null;
			Namespace = null;
		}

		public AliasRepTemplate(AliasRepTemplate copyFrom)
		{

			if (!String.IsNullOrEmpty(copyFrom.Alias))
			{
				Alias = copyFrom.Alias;
			}
			if (!String.IsNullOrEmpty(copyFrom.Namespace))
			{
				Namespace = copyFrom.Namespace;
			}
		}

		public AliasRepTemplate (string a, string u)
		{
			Alias = a;
			Namespace = u;
		}

		#region Equality
		public bool Equals (AliasRepTemplate other)
		{
			if (other == null)
				return false;
			
			return Alias == other.Alias && Namespace == other.Namespace;
		}

		public override bool Equals (object obj)
		{
			
			AliasRepTemplate temp = obj as AliasRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (AliasRepTemplate a1, AliasRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (AliasRepTemplate a1, AliasRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			return (Alias ?? String.Empty).GetHashCode () ^ (Namespace ?? String.Empty).GetHashCode ();
		}
		#endregion
		
	}
}