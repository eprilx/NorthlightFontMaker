# NorthlightFontMaker
NorthlightFontMaker is a tool used to create/edit custom bitmap fonts for Northlight engine (Alan Wake series, Quantum Break, Control and more... )

## Installation

- Download [lastest release](https://github.com/eprilx/NorthlightFontMaker/releases).
- Run `NorthlightFontMakerGUI.exe` (or run NorthlightFontMaker.exe in the cmd)

## Building from source
- **[Install .NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)**
- ``git clone --recurse-submodules https://github.com/eprilx/NorthlightFontMaker.git``
- ``dotnet build``

## Test in-game
[Verdana font and Armenian characters in Alan Wake's American Nightmare](https://github.com/eprilx/NorthlightFontMaker/blob/master/sampleImg/verdana+armenian.png?raw=true)

## Usage
❄ **Convert FNT to BINFNT (Create new BINFNT)**

```
NorthlightFontMaker --fnt2binfnt -f <path binfnt> -b <path fnt> [-o] <path export new binfnt>
```
Example: `NorthlightFontMaker --fnt2binfnt -f customer_facing.binfnt -b test.fnt -o customer_facing.binfnt.new`
1. Use [BMFont](https://www.angelcode.com/products/bmfont/) or [Hiero](https://github.com/libgdx/libgdx/wiki/Hiero) to generate FNT and PNG/DDS file.
2. The tool will automatically convert/import the PNG/DDS file.
3. For BMFont user, go to Options then Export Options, on `Font descriptor` select Text or XML.
4. From version 7 (see these version below), you must use Hiero to generate distance field font. [Here's a guide](https://github.com/libgdx/libgdx/wiki/Distance-field-fonts#generating-the-font).

**Notes**
- Increase "lineHeight" in fnt file to make font display "higher" in game and vice versa.
- Decrease "size" in fnt file to make font "bigger" and vice versa. (it also uses absolute value so |-50| > |-40|)
<img src="sampleImg/editFNT.png" />

- (Optional) Make Outline at "Options/Font settings" in BMFont or "Add Outline" in Hiero (don't use with distance field)

**List game with version:**
- Version 3: Alan Wake, Alan Wake's American Nightmare
- Version 4: Alan Wake Remastered
- Version 7: Quantum Break, Control

❄ **Convert BINFNT to FNT**
```
NorthlightFontMaker --binfnt2fnt -f <path binfnt> [-o] <path export fnt>
```
Example: ``NorthlightFontMaker --binfnt2fnt -f customer_facing.binfnt -o customer_facing.binfnt.fnt``

**Notes**
- From version 7 and above, you can't use this FNT because the yoffset when convert BINFNT to FNT is incorrect.



❄ **GUI**

<img src="sampleImg/guiScreen.png" height="280"/>

## Thanks to
- [microsoft](https://github.com/microsoft) ([texconv](https://github.com/microsoft/DirectXTex) tool)
- [Rick Gibbed](https://github.com/gibbed) ([Gibbed.IO](https://github.com/gibbed/Gibbed.IO) library)
- [romhackingvn](https://github.com/romhackingvn) (test Quantum Break)
## TODO
Make a wiki about the structure of binfnt.

## License
[MIT](LICENSE)
