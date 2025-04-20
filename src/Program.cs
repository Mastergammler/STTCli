// See https://aka.ms/new-console-template for more information
using System.Globalization;

var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var setup = new Setup();
var context = setup.CreateDbContext(args);

var factory = new CommandFactory(context);
var repl = new Repl(factory);
repl.MainLoop();