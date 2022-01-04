# Sage300UnitsExport
Export units from time editor to Sage 300

Each time Sage upgrades their software they create new folders in the GAC_32, there are four Reference files 
we need to re-point to the correct folder:

Sage.Data.CRE.Interfaces
Sage.LS1.LibraryManagment
Sage.LS1.Messaging
Sage.LS1.Messaging.Interfaces

Here is an example of the file locations from one of the above files:
C:\Windows\Microsoft.NET\assembly\GAC_32\Sage.Data.CRE.Interfaces\v4.0_20.5.0.0__3e78b2cabf12f868\Sage.Data.CRE.Interfaces.dll  (notice the folder has the new version number of Sage)

I typically just take a screen shot of the four files locations, then delete the four files and then click re-add and then browse for the new file location (you may see older versions of that folder, select version that matches current new version).

Once this is changed out I switch the version number in the UnitsExportDriver.cs (see below)

Console.WriteLine("v1.17 - Sage300 v20.5");  \\I typically just switch the sage version since code really hasn't changed v20.5

I then hit run and it compiles

Once compiled I go to the C:\Users\ktanner\git\Sage300UnitsExport\Sage300UnitsExport\bin\Debug and copy 6 files from this directory all of them start with:
'Sage300UnitsExport'  (.exe, .exe.config, .pdb, .vshost.exe, .vshost.exe.config)

Open the two config files and clear out any passwords or credentials we use
I will then copy and paste in a new folder that is named with the entire new version name  'v1.17 - Sage300 v20.5' 

Zip up the folder and place on google drive

