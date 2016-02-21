namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;

	public class IterableRepTemplate : TranslationBase, IEquatable<IterableRepTemplate>
	{

		public TypeRepRef ElementType {
			get; set;
		}

		public override string mkJava() {
			return "${expr}";
		}
		
		public IterableRepTemplate () : base()
		{
		}

		public IterableRepTemplate(String ty)
			: base()
		{
			ElementType = new TypeRepRef(ty);
		}

		public IterableRepTemplate(TypeRepTemplate parent, IterableRepTemplate copyFrom)
			: base(parent, copyFrom)
		{
			if (copyFrom != null)
			{
				ElementType = new TypeRepRef(copyFrom.ElementType);
			}
		}

		public IterableRepTemplate (String ty, string[] imps, string javaRep) : base(imps, javaRep)
		{
			ElementType = new TypeRepRef(ty);
		}


		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			if (ElementType != null)
			{
				ElementType.SubstituteInType(args);
			}
			base.Apply(args);
		}

		#region Equality

		public bool Equals (IterableRepTemplate other)
		{
			if (other == null)
				return false;
			
			return ElementType == other.ElementType && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			IterableRepTemplate temp = obj as IterableRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (IterableRepTemplate a1, IterableRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (IterableRepTemplate a1, IterableRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{	
			int hashCode = ElementType != null ? ElementType.GetHashCode() : 0;

			return hashCode ^ base.GetHashCode ();
		}
		#endregion
	}
}