# Dependencies

Don't you worry, no third party external stuff is required, but the files in this folder can depend on each other.
If you are unsure about what needs what and you don't wish to sort it out, just link to all (.cs) files in this folder and you are mostly fine.

However if you care about the size of your executables and the usage of RAM, maybe you only want to include the classes you actually need, and if they rely on each other, it's good to know what relies on what, eh?


# All files

All files will link to mkl.cs for passing through version information. The easiest way to go is just to always have that file present. If you hate that, just prefix all lines containing the MKL.Version and MKL.Lic commands with "//", but as that may cause conflicts with the github repository and can also be a pretty awful job to do, I recommend not to pain yourself and just have that file present in all projects using any of these classes :P




# File specific

Below I'll list all files relying on another class other than mkl.cs
If the list is empty or if the file is not listed, then only mkl.cs is required and nothing more.
If I forgot any, just write an issue about it, so I can update this list :P
(Can I do that with a pull request? --> Of course you can!)

