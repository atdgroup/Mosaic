Notes for the distribution of Mosaic

Set the version number in MosaicSetup Properties Version and say yes to the prompt to change the ProductCode (else you will get "A version of this product is already intalled" error on upgrade).

Build MosaicSetup to create the msi file.

On target machine .net 4.0 Client Profile is required as well as vcredist 9.0 2008 (as FreeImageAlgorithms etc have been build with this.)
Client Profile: This is the reduced set of the .net framework, this is set in Mosaic Properties Application and as a Dependency and in Prerequisites in MosaicSetup.

Have set these as prerequisites in VS here: MosaicSetup Properties Prerequisites
Must select to get prerequisites from the same location as my application to have them all packaged up or can choose to have them downloaded from somewhere.
Currently using http://users.ox.ac.uk/~atdgroup/software/bootstrapping/ to host the files and they must be in subfolders.
NB: It is "setup.exe" that does the bootstrapping so include that.

To get VC++ 2008 Runtime Libraries to appear in the Prerequisites panel of VS 2010, place the folder "vcredist_x86 - 2008" from vcredist into 
C:\Program Files\Microsoft SDKs\Windows\v7.0A\Bootstrapper\Packages


