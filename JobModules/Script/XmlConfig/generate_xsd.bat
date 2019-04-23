cd %~dp0
set dll=%1

cd ..\Assets\Weapons\
del /q /f Weapon.xsd
call xsd %dll% /type:WeaponConfigNs.WeaponConfigs
rename schema.xsd Weapon.xsd
cd ..\..\WeaponConfig

exit 0