namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[XmlType("Struct")]
	public class StructRepTemplate : ClassRepTemplate, IEquatable<StructRepTemplate>
	{

		public StructRepTemplate ()
		{
		}

		public StructRepTemplate(StructRepTemplate copyFrom)
			: base(copyFrom)
		{
		}
        
		public StructRepTemplate(string typeName)
			: base(typeName)
		{
		}

		public StructRepTemplate (string tName, string[] tParams, string[] usePath, AliasRepTemplate[] aliases, string[] inherits, List<ConstructorRepTemplate> cs, List<MethodRepTemplate> ms, List<PropRepTemplate> ps, List<FieldRepTemplate> fs, List<FieldRepTemplate> es, List<IndexerRepTemplate> ixs, List<CastRepTemplate> cts,
			string[] imports, string javaTemplate) : base(tName, tParams, usePath, aliases, inherits, cs, ms, ps, fs, es, ixs, cts,
				imports, javaTemplate)
		{
		}

		public StructRepTemplate (string tName, string[] tParams, string[] usePath, AliasRepTemplate[] aliases, string[] inherits, List<ConstructorRepTemplate> cs, List<MethodRepTemplate> ms, List<PropRepTemplate> ps, List<FieldRepTemplate> fs, List<FieldRepTemplate> es, List<IndexerRepTemplate> ixs, List<CastRepTemplate> cts)
			: base(tName, tParams, usePath, aliases, inherits, cs, ms, ps, fs, es, ixs, cts,	null, null)
		{
		}

		public override ResolveResult Resolve(String name, bool forWrite, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			return base.Resolve(name, forWrite, AppEnv,implicitCast);
		}

		#region Equality
		public bool Equals (StructRepTemplate other)
		{
			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			StructRepTemplate temp = obj as StructRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (StructRepTemplate a1, StructRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (StructRepTemplate a1, StructRepTemplate a2)
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