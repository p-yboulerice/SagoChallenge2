#!/usr/bin/env ruby
require 'fileutils'
require 'pathname'

module SagoBiz

	class FrameworkHelper
		
		
		# build accessors
		
		def build_configuration
			ENV['CONFIGURATION']
		end
		
		def build_dir(sdk)
			"#{build_root}/#{build_configuration}-#{build_sdk(sdk)}"
		end
		
		def build_sdk(sdk)
			case sdk.to_sym
			when :device
				'iphoneos'
			when :simulator
				'iphonesimulator'
			when :universal
				'iphoneuniversal'
			else
				nil
			end
		end
		
		def build_root
			ENV['BUILD_DIR']
		end
		
		def project_root
			ENV['PROJECT_DIR']
		end
		
		def project_name
			ENV['PROJECT_NAME']
		end
		
		def submodule_root
			Pathname.new("#{project_root}/../").realpath.to_s
		end
		
		
		# bundle accessors
		
		def bundle_product_name
			"#{project_name}.bundle"
		end
		
		def bundle_target_name
			"Bundle"
		end

		
		# framework accessors
		
		def framework_executable_name
			ENV['PROJECT_NAME']
		end
		
		def framework_executable_path
			"#{framework_product_path}/Versions/A/#{framework_executable_name}"
		end
		
		def framework_headers_path
			"#{framework_product_path}/Versions/A/Headers"
		end
		
		def framework_product_name
			"#{project_name}.framework"
		end
		
		def framework_product_path
			"#{build_dir(:universal)}/#{framework_product_name}"
		end
		
		def framework_resources_path
			"#{framework_product_path}/Versions/A/Resources"
		end
		
		def framework_target_name
			"Framework"
		end

		
		# library accessors
		
		def library_headers_path(sdk)
			"#{build_dir(sdk)}/Headers"
		end
		
		def library_product_name
			"#{project_name}.a"
		end
		
		def library_product_path(sdk)
			"#{build_dir(sdk)}/#{library_product_name}"
		end
		
		def library_resources_path(sdk)
			"#{build_dir(sdk)}/Resources"
		end
		
		def library_target_name
			"Library"
		end
		
		
		# methods
		
		def clean
			FileUtils.rm_rf(build_dir(:device))
			FileUtils.rm_rf(build_dir(:simulator))
			FileUtils.rm_rf(build_dir(:universal))
		end
		
		def build_bundle
			%x{
				xcodebuild \
				-project "#{project_name}.xcodeproj" \
				-sdk iphoneos \
				-target "#{bundle_target_name}" \
				-configuration "#{build_configuration}" \
				clean build CONFIGURATION_BUILD_DIR="#{build_dir(:device)}/Resources"
			}
		end
		
		def build_library

			# build library (simulator)
			%x{
				xcodebuild \
				-project "#{project_name}.xcodeproj" \
				-sdk iphonesimulator \
				-target "#{library_target_name}" \
				-configuration "#{build_configuration}" \
				clean build CONFIGURATION_BUILD_DIR="#{build_dir(:simulator)}"
			}

			# build library (device)
			%x{
				xcodebuild \
				-project "#{project_name}.xcodeproj" \
				-sdk iphoneos \
				-target "#{library_target_name}" \
				-configuration "#{build_configuration}" \
				clean build CONFIGURATION_BUILD_DIR="#{build_dir(:device)}"
			}
			
			# build library (universal)
			FileUtils.mkdir_p("#{build_dir(:universal)}/#{framework_product_name}/Versions/A")
			%x{ 
				lipo \
				"#{library_product_path(:device)}" \
				"#{library_product_path(:simulator)}" \
				-create -output "#{framework_executable_path}"
			}
			
		end
	
		def copy_headers
			FileUtils.mkdir_p(framework_headers_path)
			FileUtils.cp_r(Dir.glob("#{library_headers_path(:device)}/*"), framework_headers_path)
		end
		
		def copy_resources
			FileUtils.mkdir_p(framework_resources_path)
			FileUtils.cp_r(Dir.glob("#{library_resources_path(:device)}/*"), framework_resources_path)
		end
		
		def create_symlinks
			FileUtils.ln_sf("A","#{framework_product_path}/Versions/Current")
			FileUtils.ln_sf("Versions/Current/Headers", "#{framework_product_path}/Headers")
			FileUtils.ln_sf("Versions/Current/Resources", "#{framework_product_path}/Resources")
			FileUtils.ln_sf("Versions/Current/#{framework_executable_name}", "#{framework_product_path}/#{framework_executable_name}")
		end
		
		
		# actions
		
		def build
			clean
			build_library
			build_bundle
			copy_headers
			copy_resources
			create_symlinks
		end
		
		def build_and_copy
			build
			copy_to_project
			copy_to_submodule
		end
		
		def copy_to_project
			dst_path = "#{project_root}/Test/#{framework_product_name}"
			FileUtils.rm_rf(dst_path)
			FileUtils.cp_r(framework_product_path, dst_path)
		end
		
		def copy_to_submodule
			FileUtils.rm_rf("#{submodule_root}/#{framework_product_name}")
			FileUtils.cp_r("#{project_root}/Bindings/SBNativeBindings.h", submodule_root)
			FileUtils.cp_r("#{project_root}/Bindings/SBNativeBindings.mm", submodule_root)
			FileUtils.cp_r(framework_product_path, submodule_root);
		end
		
		
		# main
		
		def main
			action = ARGV[0].to_sym
			if self.respond_to?(action)
				self.send(action)
			else
				puts "invalid action: #{action}"
			end
		end
		
		
	end
	
end

SagoBiz::FrameworkHelper::new.main