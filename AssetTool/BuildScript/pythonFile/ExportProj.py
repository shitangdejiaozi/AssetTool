import  os
import sys
import CommonFunc

LINK_FILE="build_link_list.txt"

def clearDestProj(dest):
    if not os.path.exists(dest) :
        return
    CommonFunc.delDir(dest)


def linkProj(srcProj, destProj):
    linkDirs = CommonFunc.readListFromFile(LINK_FILE)
    print linkDirs
    for link in linkDirs :
        targetDir = os.path.join(srcProj, link)
        linkDir = os.path.join(destProj, link)
        CommonFunc.linkDir(linkDir, targetDir)
    print "link finish"

def exportProj(srcProj, destProj, platform):
    if not os.path.exists(srcProj) :
        print "srcproj not exist"
        sys.exit(-1)
    clearDestProj(destProj)
    linkProj(srcProj, destProj)

if "__main__" == __name__ :
    srcProj = CommonFunc.GetFuncArg("-srcProj")
    print srcProj
    destProj = CommonFunc.GetFuncArg("-destProj")
    print destProj
    platform = CommonFunc.GetFuncArg("-platform")
    print platform
    exportProj(srcProj, destProj, platform)
