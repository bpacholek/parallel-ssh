parallel-ssh
============

To run under Windows-based system run as every other application - requires .NET installed in the system (after Vista installed by default).
ie.
* nssh.exe

Under Linux-based and BSD-based systems please use MONO-platform to run CLR. You can obtain it from http://www.mono-project.com/. After installation of MONO you should be able to run parallel SSH by typing:

* mono nssh.exe

Application supports one optional paramater which is the path to configuration file. In case of this parameter missing default.psh is used.
The structure of the .psh file is simple - it is a CSV file with every server in new line. Parameters (from left to right)

* Name - name to use in the interface
* Host - hostname / IP
* user name - user used to log in to SSH
* password
* prompt - expected command prompt - not used at the moment (# is used)

Example:
* webserver1,webserver1.com,root,mypass123,#
* webserver2,webserver2.com,root,mypass456,#

To close the application simply kill it - type in "exit" command before.

IdeaConnect SSH client which allows simultaneous operations on multiple servers