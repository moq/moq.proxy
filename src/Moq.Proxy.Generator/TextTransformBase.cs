using System;
using System.Text;

namespace Moq.Proxy.Generator
{
	public abstract class TextTransformBase
	{
		static ToStringInstanceHelper toStringHelper = new ToStringInstanceHelper();
		StringBuilder buffer;
		bool endsWithNewline;
		string currentIndent = "";

		protected TextTransformBase ()
		{
			buffer = new StringBuilder ();
		}

		public abstract string TransformText ();

		public override string ToString () => TransformText ();

		protected void Write (string textToAppend)
		{
			if (string.IsNullOrEmpty (textToAppend))
				return;

			// If we're starting off, or if the previous text ended with a newline,
			// we have to append the current indent first.
			if (GenerationEnvironment.Length == 0 || endsWithNewline) {
				GenerationEnvironment.Append (currentIndent);
				endsWithNewline = false;
			}

			// Check if the current text ends with a newline
			if (textToAppend.EndsWith (Environment.NewLine, StringComparison.CurrentCulture))
				endsWithNewline = true;

			// This is an optimization. If the current indent is "", then we don't have to do any
			// of the more complex stuff further down.
			if (currentIndent.Length == 0) {
				GenerationEnvironment.Append (textToAppend);
				return;
			}

			// Everywhere there is a newline in the text, add an indent after it
			textToAppend = textToAppend.Replace (Environment.NewLine, Environment.NewLine + currentIndent);

			// If the text ends with a newline, then we should strip off the indent added at the very end
			// because the appropriate indent will be added when the next time Write() is called
			if (endsWithNewline) {
				GenerationEnvironment.Append (textToAppend, 0, (textToAppend.Length - currentIndent.Length));
			} else {
				GenerationEnvironment.Append (textToAppend);
			}
		}

		protected StringBuilder GenerationEnvironment => buffer;

		protected ToStringInstanceHelper ToStringHelper => toStringHelper;

		protected class ToStringInstanceHelper
		{
			public string ToStringWithCulture (object objectToConvert) => objectToConvert.ToString ();
		}
	}
}
