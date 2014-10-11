
--- PhotoSift Source Code Comments --------------------------------------------

  Language: C#
  IDE: Visual Studio 2010 Professional
  Target: DotNet Framework 2.0 on Windows
  License: GPL

  Notes:

    I need to maintain a custom version of PhotoSift with some special 
	requirements that are not suitable for the public version. This is 
	managed using "#if RLVISION ... #endif" where needed. This symbol 
	is defined in two special configurations called "RLV Debug" and 
	"RLV Release".
	
	Is Mono supported? I don't know. There are a few P/Invoke that probably
	need to be removed, but these are generally not essential. I have collected
	all Windows specific API calls in the WinAPI.cs file. Let me know if you 
	know more about Mono!

	I have tried to make the code well documented and structured. I think I
	have done a fairly good job, with one exception: The code and workflow
	managing the various zoom and view modes is not always pretty. Apologies 
	for	this. I will work on improving it in the future.

	If you modify PhotoSift and release it publicly, I would be happy to hear
	about it!

