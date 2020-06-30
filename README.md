MCMS


#### add submodule with ssh access
```
eval $(ssh-agent)
ssh-add key_path
git submodule add ssh://git@git.ligaac.ro:5022/upt/mcms.git MCMS
```