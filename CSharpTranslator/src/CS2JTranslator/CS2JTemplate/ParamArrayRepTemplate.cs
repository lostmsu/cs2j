namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;

	public class ParamArrayRepTemplate : ParamRepTemplate, IEquatable<ParamArrayRepTemplate>
	{

		public ParamArrayRepTemplate ()
		{
		}

		public ParamArrayRepTemplate(ParamArrayRepTemplate copyFrom) : base(copyFrom)
		{
		}

		public ParamArrayRepTemplate (string t, string a) : base(t, a)
		{
		}

		#region Equality
		public bool Equals (ParamArrayRepTemplate other)
		{
			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			ParamArrayRepTemplate temp = obj as ParamArrayRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (ParamArrayRepTemplate a1, ParamArrayRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (ParamArrayRepTemplate a1, ParamArrayRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
		#endregion
	}
}