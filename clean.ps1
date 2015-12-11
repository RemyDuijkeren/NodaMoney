#######################
# Delete Junk
#
#

ls -Recurse -include 'bin','obj','packages' |
  foreach { 
    remove-item $_ -recurse -force
    write-host deleted $_
}