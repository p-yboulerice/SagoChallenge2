#!/usr/bin/env ruby

# plist GitHub Project: https://github.com/patsplat/plist
require_relative 'plist'

class ExportOptions

	@@rawTemplate = "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n<plist version=\"1.0\">\n<dict>\n\t<key>method</key>\n\t<string></string>\n\t<key>teamID</key>\n\t<string></string>\n\t<key>signingStyle</key>\n\t<string></string>\n\t<key>signingCertificate</key>\n\t<string></string>\n\t<key>provisioningProfiles</key>\n\t<dict>\n\t</dict>\n\t<key>uploadSymbols</key>\n\t<true/>\n\t<key>uploadBitcode</key>\n\t<false/>\n</dict>\n</plist>\n"

	@plist

	@provisioningProfiles

	def self.rawTemplate
		@@rawTemplate
	end

	def plist
		@plist
	end

	def method=(value)
		@plist['method'] = value
	end

	def method
		@plist['method']
	end

	def teamID=(value)
		@plist['teamID'] = value
	end

	def teamID
		@plist['teamID']
	end

	def shouldAutoSign=(value)
		@plist['signingStyle'] = value ? 'automatic' : 'manual'
	end

	def shouldAutoSign
		@plist['signingStyle'] == 'automatic'
	end

	def signingCertificate=(value)
		@plist['signingCertificate'] = value
	end

	def signingCertificate
		@plist['signingCertificate']
	end

	def provisioningProfiles=(value)
		provisioningProfiles = value
		@plist['provisioningProfiles'] = provisioningProfiles
	end

	def uploadBitcode=(value)
		if !!value == value
			@plist['uploadBitcode'] = value
		end
	end

	def uploadBitcode
		@plist['uploadBitcode']
	end

	def uploadSymbols=(value)
		if !!value == value
			@plist['uploadSymbols'] = value
		end
	end

	def uploadSymbols
		@plist['uploadSymbols']
	end

	def initialize
		@plist = Plist.parse_xml(self.class.rawTemplate)
		self.signingCertificate = 'iOS Distribution: POSSIBLE Mobile'
	end

end