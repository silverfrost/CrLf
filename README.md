# CrLf
Utility for converting line endings. 

Usage: crlf file [/w] [/m] [/l] [/o:file1]

Giving a file only will report the line ending.

/w convert to Windows CRLF ending

/m convert to Mac line feed ending (synonyms: /10 /lf /n)

/l convert to Linux carriage return ending (synonyms: /13 /cr /r)

The given file **will by overwritten** unless you give a destination file via /o



