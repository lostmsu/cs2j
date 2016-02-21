namespace Twiglet.CS2J.Translator.TypeRep
{
	using System.Collections.Generic;

	public interface IApplyTypeArgs
	{
		// Instantiate type arguments "in-situ"
		void Apply(Dictionary<string,TypeRepTemplate> args);
	}
}