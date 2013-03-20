Sublimerge
==========

Sublimerge is a Sublime Text 2 plugin which allows to diff and merge files DIRECTLY in the editor using graphical interface for that purpose.

If file is under GIT or SVN it is now possible to compare it with its other revisions. This is experimental feature and can be simply
turned off via package settings (vcs_support). Please report any problems with this feature. Requires svn or git to be installed.

![Sublimerge](http://cloud.github.com/downloads/borysf/Sublimerge/Screenshot2.png "Sublimerge")

Default key bindings (platform independent)
------------------------------------------

`[ctrl]+[alt]+[d]` - open files menu to select the file you wish diff the current file to 

`[ctrl]+[alt]+[,]` - merge the change from right to left  
`[ctrl]+[alt]+[.]` - merge the change from left to right  

`[ctrl]+[alt]+[/] + [ctrl]+[alt]+[,]` - merge all changes from right to left  
`[ctrl]+[alt]+[/] + [ctrl]+[alt]+[.]` - merge all changes from left to right  

`[ctrl]+[alt]+[=]` or `[ctrl]+[alt]+[pagedown]` - go to the next difference  
`[ctrl]+[alt]+[-]` or `[ctrl]+[alt]+[pageup]` - go to the previous difference  


Access from context menus
-------------------------

You can access Sublimerge from the following context menus:
- selected file(s) in sidebar
- editor's text area
- file's tab


Installation
------------

If you are using Will Bond's Package Control, you can easily install Sublimerge with `Package Control: Install Package`.
Otherwise you can simply clone this repo in Sublime Text 2' Packages directory.

Donation
--------

If you find this plugin useful, you can say "thanks" by Donation: http://borysforytarz.pl/sublimerge.html :)

--
Because this is my first Sublime Text 2 plugin and the first Python code I have ever written, probably some things could be done
better - any feedback is welcome!

https://github.com/borysf/Sublimerge  
borys.forytarz@gmail.com  
http://borysforytarz.pl  