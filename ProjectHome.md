### OOXML Crypto Stream ###
Create, open and save password-protected Microsoft Office 2007 files.

Works with Office 2007 files and encryption scheme only (.docx, .xlsx, .pptx). No support for older formats.

Includes slightly modified [ExcelPackage](http://code.google.com/p/ooxmlcrypto/) for accessing Excel 2007 .xlsx files.

**Download the [binary release](http://ooxmlcrypto.googlecode.com/files/OfficeOpenXmlCrypto-bin0.1.zip) (68k).**

Written in C# / .Net 3.0

### Sample Code ###
```
using (OfficeCryptoStream stream = OfficeCryptoStream.Open("a.xlsx", "password"))
{
    // Do stuff (e.g. create System.IO.Packaging.Package or 
    // ExcelPackage from the stream, make changes and save)

    // Change the password (optional)
    stream.Password = "newPassword";

    // Encrypt and save the file
    stream.Save();
}
```

### Details ###
Office 2007 files (Word 2007 .docx, Excel 2007 .xlsx, Power Point 2007 .pptx)  are zip files containing XML. They are relatively easy read and modify.

Microsoft's [Package class](http://msdn.microsoft.com/en-us/library/system.io.packaging.package.aspx) provides access to plaintext Office 2007 files. This project adds support  for password-protected files (OLE compound files with Microsoft's proprietary encryption applied).

### Pieces ###
This project does little or no heavy lifting. It wraps together several open-source pieces in a single, easy-to-use package.
  * [ExcelPackage](http://excelpackage.codeplex.com/) for accessing .xlsx spreadsheets. Modified to accept stream in the constructor.
  * [Lyquidity OoxmlCrypto](http://www.lyquidity.com/devblog/?p=35) open source sample code for encrypting/decrypting the files. Based on info from [David LeBlanc](http://blogs.msdn.com/david_leblanc/) from Microsoft.
  * [NPOI](http://npoi.codeplex.com/) project for access to OLE2 Compount Container files (those are legacy Office format, used to wrap encrypted Office 2007 stream). This replaces Lyquidity's properietary container access library.