NDesk.Options
=============

NDesk.Options is a program option parser for C#.

See: http://www.ndesk.org/Options

Overview:
--------

It takes advantage of C# 3.0 features such as collection initializers and
lambda delegates to provide a short, concise specification of the option 
names to parse, whether or not those options support values, and what to do 
when the option is encountered.  It's entirely callback based:

	var verbose = 0;
	var show_help = false;
	var names = new List<string> ();

	var p = new OptionSet () {
		{ "v|verbose", v => { if (v != null) ++verbose; } },
		{ "h|?|help",  v => { show_help = v != null; } },
		{ "n|name=",   v => { names.Add (v); } },
	};

However, C# 3.0 features are not required, and can be used with C# 2.0:

	int          verbose   = 0;
	bool         show_help = false;
	List<string> names     = new List<string> ();

	OptionSet p = new OptionSet ()
	  .Add ("v|verbose", delegate (string v) { if (v != null) ++verbose; })
	  .Add ("h|?|help",  delegate (string v) { show_help = v != null; })
	  .Add ("n|name=",   delegate (string v) { names.Add (v); });
