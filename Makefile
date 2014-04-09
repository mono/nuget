CHMOD ?= chmod
CP ?= cp
CURL ?= curl
INSTALL ?= install
MKDIR ?= mkdir
MODE ?= Debug
PREFIX ?= /usr/local
SED ?= sed

ifeq ($(MODE), Debug)
	override mono_opt = --debug
else
	override mono_opt =
endif

.PHONY: all clean install uninstall

all: bin/nuget lib/nuget/NuGet.exe

clean:
	$(RM) bin/nuget
	$(RM) lib/nuget/NuGet.exe

bin/nuget: nuget.in
	$(MKDIR) -p bin
	$(SED) -e 's,@''bindir@,$(PREFIX)/bin,' -e 's,@''mono_instdir@,$(PREFIX)/lib/mono,' nuget.in > bin/nuget
	$(CHMOD) +x bin/nuget

# lib/nuget/NuGet.exe:

# 	$(CURL) -L http://nuget.org/nuget.exe -o lib/nuget/NuGet.exe

lib/nuget/NuGet.exe:
	./build.sh
	$(MKDIR) -p lib/nuget
	cp $(PWD)/src/CommandLine/bin/Release/* lib/nuget/

install:
	$(INSTALL) -m755 -d $(PREFIX)
	$(INSTALL) -m755 -d $(PREFIX)/bin
	$(INSTALL) -m755 -d $(PREFIX)/lib/mono/nuget
	$(INSTALL) -m755 bin/nuget $(PREFIX)/bin
	$(INSTALL) -m755 lib/nuget/NuGet.exe $(PREFIX)/lib/mono/nuget
	$(INSTALL) -m755 lib/nuget/NuGet.Core.dll $(PREFIX)/lib/mono/nuget
	$(INSTALL) -m755 lib/nuget/Microsoft.Web.XmlTransform.dll $(PREFIX)/lib/mono/nuget

uninstall:
	$(RM) $(PREFIX)/lib/nuget/nuget.exe
	$(RM) $(PREFIX)/bin/nuget
