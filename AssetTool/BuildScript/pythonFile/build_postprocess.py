import CommonFunc
import  os
import  sys

def build_widow(package, updatePath):
    curDir = CommonFunc.getDir(package)
    resDir = CommonFunc.findDir(curDir, "StreamingAssets")
    CommonFunc.CopyDir(updatePath, resDir)

def build_android(package, updatePath):
    return

if "__main__" == __name__ :
    package = CommonFunc.GetFuncArg("-package")
    if not os.path.exists(package) :
        print "package not find"
        sys.exit(-1)

    platform = CommonFunc.GetFuncArg("-platform")
    if platform == "":
        print "platform not find"
        sys.exit(-1)
    updatePath = CommonFunc.GetFuncArg("-updatePath")
    if not os.path.exists(updatePath) :
        print "updatepath not find"
        sys.exit(-1)

    if platform == "window"  :
        build_widow(package, updatePath)
    elif platform == "android":
        build_android(package, updatePath)