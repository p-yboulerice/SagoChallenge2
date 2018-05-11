#!/usr/bin/env ruby

# plist GitHub Project: https://github.com/patsplat/plist
require_relative 'plist'






class ProvisioningProfile

	@@provisioning_profile_path = "#{Dir.home}/Library/MobileDevice/Provisioning Profiles"

	@@dump_provisioning_profile_command = "security cms -D -i"

	@full_file_name

	@file_name

	@file_extension

	@provisioning_profile_name

	@application_identifier_prefix

	@team_id

	@app_id_name

	@get_task_allow

	@has_devices

	@is_enterprise

	def full_file_name
		@full_file_name
	end

	def file_name
		@file_name
	end

	def file_extension
		@file_extension
	end

	def provisioning_profile_name
		@provisioning_profile_name
	end

	def application_identifier_prefix
		@application_identifier_prefix
	end

	def team_id
		@team_id
	end

	def app_id_name
		@app_id_name
	end

	def get_task_allow
		@get_task_allow
	end

	def has_devices
		@has_devices
	end

	def is_enterprise
		@is_enterprise
	end

	def self.find_match( targeted_provisioning_profile_name )

		Dir
		.entries(@@provisioning_profile_path)
		.reject { |currentFileName| 
			# return true to reject, return false to keep
			!File.file?("#{@@provisioning_profile_path}/#{currentFileName}") || (File.extname(currentFileName) != ".provisioningprofile" && File.extname(currentFileName) != ".mobileprovision")
		}
		.each { |currentFileName| 

			# Create a command for dumping provisioning profile info in command-line.
			command = "#{@@dump_provisioning_profile_command} '#{@@provisioning_profile_path}/#{currentFileName}'"

			# Execute the command and save to a variable as a raw data.
			rawPlist = `#{command}`

			# Parse the raw data to a Plist object.
			parsedPlist = Plist.parse_xml(rawPlist)

			fullFileName = currentFileName
			fileName = File.basename(currentFileName)
			fileExtension = File.extname(currentFileName)
			provisioningProfileName = parsedPlist['Name']
			applicationIdentifierPrefix = parsedPlist['ApplicationIdentifierPrefix'].join("")
			appIdName = parsedPlist['AppIDName']
			getTaskAllow = parsedPlist['Entitlements']['get-task-allow']
			hasDevices = parsedPlist['ProvisionedDevices'].kind_of?(Array)
			isEnterprise = !parsedPlist['ProvisionsAllDevices'].nil? ? parsedPlist['ProvisionsAllDevices'] : false

			if provisioningProfileName == targeted_provisioning_profile_name	

				return fullFileName

			end

		}

	end

	def initialize( provisioningProfileFileName )

		# Create a command for dumping provisioning profile info in command-line.
		command = "#{@@dump_provisioning_profile_command} '#{@@provisioning_profile_path}/#{provisioningProfileFileName}'"

		# Execute the command and save to a variable as a raw data.
		rawPlist = `#{command}`

		# Parse the raw data to a Plist object.
		parsedPlist = Plist.parse_xml(rawPlist)

		@full_file_name = provisioningProfileFileName
		@file_name = File.basename(provisioningProfileFileName)
		@file_extension = File.extname(provisioningProfileFileName)
		@provisioning_profile_name = parsedPlist['Name']
		@application_identifier_prefix = parsedPlist['ApplicationIdentifierPrefix'].join("")
		@team_id = parsedPlist['TeamIdentifier'].join("")
		@app_id_name = parsedPlist['AppIDName']
		@get_task_allow = parsedPlist['Entitlements']['get-task-allow']
		@has_devices = parsedPlist['ProvisionedDevices'].kind_of?(Array)
		@is_enterprise = !parsedPlist['ProvisionsAllDevices'].nil? ? parsedPlist['ProvisionsAllDevices'] : false

	end

end

