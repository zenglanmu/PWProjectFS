﻿## 挂载pw目录为本地路径
基于dokany的User-Mode filesystem driver实现
将对pw的访问模拟成文件系统访问

## TODO
 - [x] 挂载成盘符，显示容量信息
 - [x] 浏览目录结构
 - [x] 挂载pw上根目录
 - [x] 文件系统内文件夹移动
 - [x] 文件系统内文件移动
 - [x] 文件系统内文件复制
 - [x] 跨文件系统内文件夹复制
 - [x] 跨文件系统内文件复制
 - [x] 文件删除
 - [x] 文件夹删除（有锁定的可能有问题）
 - [ ] 文件夹重命名
 - [ ] 新建目录 （可新建，但是新建完不支持改名）
 - [x] 文件重命名
 - [x] 新建文件
 - [ ] 设置文件和文件夹权限。对于只能只读的文件，设置只读权限（只读包括pw权限上只读，以及锁定或者最终状态等）
 - [x] 文件修改保存(test wps)
 - [x] 关闭后释放pw上文件占用，清空本地工作目录的缓存文件
 - [ ] 长时间登陆退出后重新登陆
 - [ ] 手动卸载

 ## know issues:
 1. windows打开大小为0的文件时，没走read。导致想编辑但没检出锁定
 2. 文件操作完，比如复制完需要手动刷新才会更新列表信息
 3. 有些文件夹/文件命名在projectwise上是合法的，但在windows中不合法。比如 /字符
 4. 挂载根目录的情况时，文件不支持移动/创建在根目录
 5. 记事本编辑文本文件时，保存会提示找不到文件
 6. 记事本打开文件时，会走只读模式。而pw端没有检出，如果再进一步保存则出错（反正保存也会提示找不到文件）
 7. wps保存文件，因为会创建一堆临时文件，导致速度明显偏慢
 8. AutoCAD保存文件，会释放文件占用，并打开临时文件。同时将数据写入bak文件，原始文件大小为空。并且原来的文件变成只读（为啥这些软件保存都不直接存。。）