## CONTRIBUTING.md

感谢所有社区贡献者，为助力完善`Tinper`全系列产品，本篇针对*bug反馈* & *pull request流程*作出说明

* [Bug反馈](#bug)

* [Pull Request流程图示](#pull)


<h3 id="bug">Bug反馈</h3>

1.提交途径

* 推荐使用issue,

  >  简单，拖拽即可上传截图，方便反馈存档，利于帮助其他存在问题的人，避免重复问题

 2.提交内容

针对存在问题反馈不够明确，建议提交包含以下内容，目前提供了一个[在线模板可供参考](https://github.com/iothua/MFrameworkCore/issues/new?template=----.md)，个人填写建议包含以下内容:

```
**问题描述**
（描述一下问题）

**生产环境** 
- 浏览器及版本： 
- 演示地址: 
- 浏览器报错: 
- 截图:
```

<h3 id="pull">Pull Request流程图示</h3>

以 [MFrameworkCore](https://github.com/iothua/MFrameworkCore) 仓库为例：

1. Fork仓库到个人Respository目录：进入仓库，点击`Fork`

   ![Fork](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/fork.jpg)

2. Clone到本地

   ```
   $ git clone git@github.com:iothua/MFrameworkCore.git
   ```

3. 创建分支

   ```
   $ git checkout -b fixer
   ```

4. 修改后提交

   ```
   $ git add .
   $ git commit -m "fix:some bug"
   $ git push origin fixer
   ```

5. 提交`pull request`:登陆github,进入`fork`后的仓库，切换到新提交的`fixer`分支，点击右侧绿色按钮`Compare& pull request`

   ![pull](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/compare.jpg)

6. 添加注释信息，确认提交

   ![commit](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/pull%20request.jpg)
   ​
7. 最终我会将你上传的，进行合并
   ![merge](https://github.com/iothua/MFrameworkCore/blob/develop/WikiPng/merge.jpg)
