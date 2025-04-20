// See https://aka.ms/new-console-template for more information
using System.Globalization;

var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var setup = new Setup();
var context = setup.CreateDbContext(args);
var repository = new SttRepository(context);

var repl = new Repl(context, repository);
repl.MainLoop();
