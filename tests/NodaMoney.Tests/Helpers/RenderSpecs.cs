using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace NodaMoney.Tests.Helpers
{
    public class RenderSpecs
    {
        [Fact]
        public void Rendering()
        {
            Render("", Console.Out);
        }

        [Fact]
        public void RenderAllSpecs()
        {
            using (var stream = File.Open(@"Specs.txt", FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                Render("", writer);
            }
        }

        private void Render(string withinNamespace, TextWriter output)
        {
            var specs = (from type in this.GetType().Assembly.GetTypes()
                         where type.Namespace != null &&
                               type.Namespace.StartsWith(withinNamespace) //&&
                                                                          //type.GetCustomAttributes(true).OfType<TestClassAttribute>().Any()
                         from method in type.GetMethods()
                         where (method.GetCustomAttributes(true).OfType<FactAttribute>().Any() ||
                                 method.GetCustomAttributes(true).OfType<TheoryAttribute>().Any()
                               ) &&
                               method.Name.StartsWith("When")
                         orderby type.Namespace, type.Name
                         select new
                         {
                             Type = type,
                             Method = method,
                             Phrase = method.Name,
                             When = ToPhrase(method.Name.Substring(0, method.Name.IndexOf('_'))),
                             Then = ToPhrase(method.Name.Substring(method.Name.IndexOf('_') + 1)),
                         })
                        .GroupBy(x => x.Type)
                        .OrderBy(x => x.Key.FullName)
                        .GroupBy(x => x.Key.IsNested ? x.Key.DeclaringType.Name : x.Key.Namespace);

            foreach (var ns in specs)
            {
                output.WriteLine(new string('-', 50));
                output.WriteLine(ToPhrase(ns.Key.Split('.').Last(), false));

                foreach (var context in ns)
                {
                    output.WriteLine("    " + ToPhrase(context.Key.Name));
                    foreach (var spec in context.OrderBy(spec => spec.When).ThenBy(spec => spec.Then))
                    {
                        output.WriteLine("        " + spec.When + ", " + spec.Then);
                        // Console.WriteLine("\t" + spec.Phrase);
                    }
                }
            }
        }

        private static string ToPhrase(string pascalCasedPhrase)
        {
            return ToPhrase(pascalCasedPhrase, true);
        }

        private static string ToPhrase(string pascalCasedPhrase, bool toLower)
        {
            var builder = new StringBuilder();
            builder.Append(pascalCasedPhrase.First());

            for (int i = 1; i < pascalCasedPhrase.Length; i++)
            {
                if (Char.IsUpper(pascalCasedPhrase[i]))
                    builder.Append(" ");

                builder.Append(pascalCasedPhrase[i]);
            }

            var phrase = builder.ToString();

            if (toLower)
            {
                phrase = phrase.ToLower(CultureInfo.CurrentCulture);
                // Make only When and Then upper case
                phrase = phrase.Replace("given", "Given").Replace("when", "When").Replace("then", "Then");
            }

            return phrase;
        }
    }
}
