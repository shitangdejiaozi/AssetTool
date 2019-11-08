#encoding=utf-8
import os
import sys
import shutil

def createDir(dir):
    os.makedirs(dir)

#会把软连接的源文件删除
def delDir(dir):
    if os.path.exists(dir):
        shutil.rmtree(dir)
    print "del dir" + dir

def forceDelDir(dir):
    if not os.path.exists(dir) :
        return
    dir =  dir.replace("/", os.sep)
    cmd = "rd /s /q \"%s\"" % (dir)
    os.system(cmd)

def forceDelFile(file):
    if not os.path.exists(file) :
        return
    file =  file.replace("/", os.sep)
    cmd = "del \"%s\"" % (file)
    os.system(cmd)

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
    print destDir
    if not os.path.exists(dir):
        createDir(dir)
    cmd = "mklink /d \"%s\" \"%s\"" % (linkDir, destDir)
    os.system(cmd)


def GetFuncArg(arg):
    for i in range(len(sys.argv)):
        if sys.argv[i] == arg :
            if i + 1 < len(sys.argv):
                return sys.argv[i+1]
            else :
                return ""
    return ""

