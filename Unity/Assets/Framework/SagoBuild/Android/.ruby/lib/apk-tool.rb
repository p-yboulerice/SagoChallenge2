#!/usr/bin/env ruby

def get_apktool_disassemble_command apkPath, targetPath
	return "apktool d -f '#{apkPath}' -o '#{targetPath}'"
end

def get_apktool_build_command disassembledApkPath
	return "apktool b " + "'#{disassembledApkPath}'"
end

def get_jarsigner_command keystorePath, keystoreAlias, storePass, keyPass, apkPath
	return "jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore '#{keystorePath}' -storepass '#{storePass}' -keypass '#{keyPass}' '#{apkPath}' #{keystoreAlias}"
end

def get_verify_jar_command keystorePath, apkPath
	return "jarsigner -verify -verbose -keystore '#{keystorePath}' '#{apkPath}'"
end

def get_zipalign_command zipAlignPath, apkPath
  filePath = File.dirname(apkPath)
  fileName = File.basename(apkPath, ".*")
  fileExtension = File.extname(apkPath)
  alignedApkPath = "#{filePath}/#{fileName}.aligned#{fileExtension}"
  return "'#{zipAlignPath}' -v -p 4 '#{apkPath}' '#{alignedApkPath}'"
end

def get_verify_zipalign_command zipAlignPath, apkPath
	return "'#{zipAlignPath}' -c 4 '#{apkPath}'"
end