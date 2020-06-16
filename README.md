﻿﻿﻿﻿Information_Content_Security_Lab
================================

# 信息内容安全实验

## 基于微博平台的动态网页内容识别及控制

初步构想实现一个微型客户端，利用爬虫手段实现**微博页面的信息捕获**，**协议分析**以及一些其他的附加功能（待定），利用主流的字符串匹配算法实现关键字匹配，提供一些热点追踪之类的功能，安全管理方面做到一定程度管控（未设想好具体实现）。

- 语言：C \#（c sharp）


- 软件：Visual Studio， Vscode

- 数据库 ： MySQL8.0


## Data_Process_System
数据处理系统：

实现网络信息获取，HTML文件解析，原始网页存储，博文内容存储，敏感词过滤等功能

项目用Visual Studio Code建立，稍微百度一下使用即可上手不难，初次尝试C sharp 语言，因此push的时候不敢删除相关文件，测试最好还是新建项目将代码拷进去。

html解析工具为AngleSharp，详见官网[AngleSharp](https://anglesharp.github.io/)官方文档以及NuGet命令，上手不难，简化正则表达式解析，还在熟悉中。

关键字解析部分利用微博api，模板形式详见（Data_Stored/api.txt)或Spider.search_for_keyword()

获取到的数据为JSON形式，用JObject解析写入csv文件，[Get Newtonsoft.Json -NuGet官网](https://www.nuget.org/packages/Newtonsoft.Json/#dotnet-cli)

文件目录：

 1.数据储备于Data_Stored文件夹下，其中：

- data文件夹下是原始报文（.html）
- csv文件夹下是格式化数据，包括用户，博文和管控过程中过滤掉的博文
- Log文件夹下是系统日志（命名“yyyy-mm-dd.txt”）
- json文件夹下一步加工之后的格式化JSON文本
- api.txt 是爬虫程序中用到的一系列api
- Cookie是实时获取的微博网站cookie
- sensitive_words是程序预设敏感词列表
				
2.代码部分分为三部分：

- 有关数据处理的辅助类和协议解析相关函数在data_process文件夹下
- 有关控管操作的相关类在secure文件夹下
- 有关爬虫操作的相关类在spider文件夹下
- 有关测试相关类在test文件夹下


## User_Interface
用户接口：

用户界面设计采用Windows Form的形式，设计了一个Windows平台的微博浏览应用，支持微博关键字搜索，实现部分统计功能，用户可以通过检索到的热点话题或者关键字相关博文的URL访问原始网页（支持复制），同时也可以在系统提供的浏览平台上查看相关微博。







