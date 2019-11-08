#encoding=utf-8
import  os
import sys
import CommonFunc

LINK_FILE="build_link_list.txt"
STREAMING_ASSETS="Assets/StreamingAssets"
PROJECTSET="ProjectSettings"
#有些文件需要调整的， 需要移动到StreamingAssets下，打包到apk中，按需添加
CopyDict = {"Assets/StreamingAssets":"", "ProjectSettings":"","Assets/Resources/data":"Assets/StreamingAssets/data"}

def clearDestProj(dest, isClearLib):
    if not os.path.exists(dest) :
        return
    if isClearLib :
        CommonFunc.forceDelDir(dest)
    else :
        dirs = os.listdir(dest)
        for d in dirs :
            if d != "Library" :
                path = os.path.join(dest, d)
                if os.path.isfile(path) :
                    CommonFunc.forceDelFile(path)
                else :
                    CommonFunc.forceDelDir(path)

#拷贝一些需要更新，但是不需要打ab的数据， 需要移动到Streamingasset下； 后续需要修改的目录
def CopyProj(srcProj, desetProj):
    for src, dest in CopyDict.items() :
        if dest == "":
            dest = src
        CommonFunc.CopyDir(os.path.join(srcProj, src), os.path.join(destProj, dest))

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
    clearDestProj(destProj, True)
    return
    linkProj(srcProj, destProj)
    CopyProj(srcProj, destProj)

if "__main__" == __name__ :
    srcProj = CommonFunc.GetFuncArg("-srcProj")
    print srcProj
    destProj = CommonFunc.GetFuncArg("-destProj")
    print destProj
    platform = CommonFunc.GetFuncArg("-platform")
    print platform
    exportProj(srcProj, destProj, platform)
