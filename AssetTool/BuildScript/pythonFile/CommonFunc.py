import os
import sys
import shutil

def createDir(dir):
    os.makedirs(dir)

def delDir(dir):
    if os.path.exists(dir):
        shutil.rmtree(dir)

def getDir(path):
    dir = os.path.dirname(path)
    dir = dir.replace("\\", "/")
    return dir

def trimStr(s):
    s = s.rstrip(' ')
    s = s.rstrip('\n')
    s = s.rstrip("\r")
    return s

def readListFromFile(file):
    lines = []
    fs = open(file, 'r')
    lines = fs.readlines()
    for i in range(len(lines)):
        lines[i] = trimStr(lines[i])
    fs.close()
    return lines

def linkDir(linkDir, destDir):
    if not os.path.exists(destDir):
        return
    dir = getDir(linkDir)
    if not os.path.exists(dir):
        createDir(dir)
    cmd = "mklink /d \"%s\" \"%s\"" % (linkDir, destDir)
    print cmd
    os.system(cmd)


def GetFuncArg(arg):
    for i in range(len(sys.argv)):
        if sys.argv[i] == arg :
            if i + 1 < len(sys.argv):
                return sys.argv[i+1]
            else :
                return ""
    return ""

