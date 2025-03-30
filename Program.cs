// See https://aka.ms/new-console-template for more information
using System.Globalization;

var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var setup = new Setup();
var context = setup.CreateDbContext(args);

var repl = new Repl(context);
repl.MainLoop();
