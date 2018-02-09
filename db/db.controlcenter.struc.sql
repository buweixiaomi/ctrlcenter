CREATE DATABASE  IF NOT EXISTS `ctrlcenterusing` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `ctrlcenterusing`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: ctrlcenterusing
-- ------------------------------------------------------
-- Server version	5.5.53-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cmdargument`
--

DROP TABLE IF EXISTS `cmdargument`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cmdargument` (
  `cmdId` int(11) NOT NULL,
  `argIndex` int(11) NOT NULL DEFAULT '0',
  `argValue` varchar(400) NOT NULL,
  `containConfig` int(11) NOT NULL DEFAULT '0' COMMENT '是否包含参数',
  PRIMARY KEY (`cmdId`,`argIndex`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='命令参数';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `command`
--

DROP TABLE IF EXISTS `command`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `command` (
  `cmdId` int(11) NOT NULL AUTO_INCREMENT,
  `codeName` varchar(100) NOT NULL,
  `title` varchar(100) DEFAULT NULL,
  `createTime` datetime NOT NULL,
  `getTime` datetime DEFAULT NULL,
  `preExecuteTime` datetime DEFAULT NULL,
  `completeTime` datetime DEFAULT NULL,
  `completeState` int(11) NOT NULL DEFAULT '0' COMMENT '0 待执行 1执行中  2执行完成 -1 执行失败',
  `completeMessage` text,
  `completeError` text,
  `serverId` int(11) NOT NULL,
  `state` int(11) NOT NULL DEFAULT '0',
  `groupKey` varchar(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`cmdId`)
) ENGINE=InnoDB AUTO_INCREMENT=419 DEFAULT CHARSET=utf8 COMMENT='命令表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `commandtarget`
--

DROP TABLE IF EXISTS `commandtarget`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `commandtarget` (
  `cmdTargetId` int(11) NOT NULL AUTO_INCREMENT,
  `cmdType` varchar(100) NOT NULL,
  `targetId` int(11) NOT NULL,
  `cmdId` int(11) NOT NULL,
  `remark` varchar(200) NOT NULL DEFAULT '',
  `sourceId` int(11) NOT NULL,
  PRIMARY KEY (`cmdTargetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cusservice`
--

DROP TABLE IF EXISTS `cusservice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cusservice` (
  `cusServiceId` int(11) NOT NULL AUTO_INCREMENT,
  `cusId` int(11) NOT NULL,
  `title` varchar(200) NOT NULL,
  `serviceDesc` text,
  `serviceTime` datetime NOT NULL,
  `serviceMan` text,
  `workItemId` int(11) NOT NULL DEFAULT '0' COMMENT '关联的工作项ID',
  `serviceCharge` decimal(10,0) DEFAULT NULL,
  `state` int(11) DEFAULT NULL COMMENT '0 正常 -1删除',
  `createManagerId` int(11) NOT NULL,
  `createManagerName` varchar(100) NOT NULL,
  `createTime` datetime NOT NULL,
  `remark` varchar(400) NOT NULL DEFAULT '',
  `serviceType` int(11) NOT NULL DEFAULT '0' COMMENT ' 服务类型 0:其它  1Bug修改 2功能调整  3新需求',
  PRIMARY KEY (`cusServiceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customer`
--

DROP TABLE IF EXISTS `customer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customer` (
  `cusId` int(11) NOT NULL AUTO_INCREMENT,
  `customerName` varchar(200) NOT NULL DEFAULT '',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT '0 待上线，1服务中  2已下线 -1已删除',
  `remark` text,
  `webDomains` varchar(500) NOT NULL DEFAULT '',
  `createTime` datetime NOT NULL,
  `updateTime` datetime DEFAULT NULL,
  `cusNo` varchar(100) NOT NULL DEFAULT '',
  `submitTime` datetime DEFAULT NULL,
  `serverOfType` int(11) NOT NULL DEFAULT '0' COMMENT '0 未定  1公司 2自动 3复合',
  `contractNo` varchar(50) NOT NULL DEFAULT '',
  `contractRemark` text,
  `contractBeginTime` datetime DEFAULT NULL,
  `contractEndTime` datetime DEFAULT NULL,
  `serverRemark` text,
  `customFunction` text,
  `tag` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`cusId`)
) ENGINE=InnoDB AUTO_INCREMENT=114 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customerlinkmanager`
--

DROP TABLE IF EXISTS `customerlinkmanager`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customerlinkmanager` (
  `cuslinkId` int(11) NOT NULL AUTO_INCREMENT,
  `cusId` int(11) NOT NULL,
  `managerId` int(11) NOT NULL,
  `title` varchar(100) NOT NULL,
  `remark` varchar(400) NOT NULL DEFAULT '',
  PRIMARY KEY (`cuslinkId`)
) ENGINE=InnoDB AUTO_INCREMENT=199 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feedback`
--

DROP TABLE IF EXISTS `feedback`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `feedback` (
  `feedbackId` int(11) NOT NULL AUTO_INCREMENT COMMENT '反馈ID',
  `title` varchar(200) NOT NULL COMMENT '标题',
  `content` text COMMENT '内容',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT '-1 已删除  0 待审核 1不与处理 2处理中  3处理完成 ',
  `createTime` datetime NOT NULL COMMENT '创建时间',
  `cusId` int(11) DEFAULT NULL COMMENT '客户iD，如果没有，可填null或0',
  `cusName` varchar(100) DEFAULT NULL COMMENT '客户名称',
  `managerId` int(11) NOT NULL COMMENT '建档人ID',
  `managerName` varchar(100) NOT NULL COMMENT '建档人名称',
  `workItemId` int(11) DEFAULT NULL COMMENT '生成工作项后的ID',
  `lastProcessTime` datetime DEFAULT NULL COMMENT '上次操作时间',
  `remark` text COMMENT '备注',
  `checkManagerId` int(11) DEFAULT NULL COMMENT '审核人编号',
  `checkManagerName` varchar(100) DEFAULT NULL COMMENT '审核人名称',
  `checkRemark` text COMMENT '审核说明',
  `checkTime` datetime DEFAULT NULL COMMENT '审核时间',
  `feedbackType` int(11) NOT NULL COMMENT '反馈类型  0其它问题 1Bug  2新需求 3功能调整 4建议',
  `fromSource` int(11) NOT NULL DEFAULT '0' COMMENT '反馈来源  0后台添加   1客户反馈',
  PRIMARY KEY (`feedbackId`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COMMENT='反馈表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `manager`
--

DROP TABLE IF EXISTS `manager`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `manager` (
  `managerId` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(60) NOT NULL,
  `subName` varchar(100) NOT NULL DEFAULT '',
  `loginName` varchar(100) NOT NULL DEFAULT '',
  `loginPwd` varchar(100) DEFAULT NULL,
  `allowLogin` int(11) NOT NULL DEFAULT '0' COMMENT '0 不允许 1允许',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT '0正常  1已冻结  -1已删除',
  `createTime` datetime NOT NULL,
  `lastLoginTime` datetime DEFAULT NULL,
  `updateTime` datetime DEFAULT NULL,
  `remark` varchar(400) NOT NULL DEFAULT '',
  PRIMARY KEY (`managerId`)
) ENGINE=InnoDB AUTO_INCREMENT=58 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `managertaglink`
--

DROP TABLE IF EXISTS `managertaglink`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `managertaglink` (
  `managertaglinkId` int(11) NOT NULL AUTO_INCREMENT,
  `managerId` int(11) NOT NULL,
  `usertagId` int(11) NOT NULL,
  PRIMARY KEY (`managertaglinkId`)
) ENGINE=InnoDB AUTO_INCREMENT=65 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `operationlog`
--

DROP TABLE IF EXISTS `operationlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `operationlog` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `OperationContent` text COMMENT '操作内容',
  `OperationName` varchar(100) DEFAULT NULL COMMENT '操作人',
  `Createtime` datetime DEFAULT NULL,
  `OperationTitle` varchar(200) DEFAULT NULL COMMENT '标题',
  `Module` varchar(200) DEFAULT NULL COMMENT '模块',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2954 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `project`
--

DROP TABLE IF EXISTS `project`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `project` (
  `projectId` int(11) NOT NULL AUTO_INCREMENT,
  `CodeName` varchar(100) NOT NULL,
  `Title` varchar(100) NOT NULL,
  `State` int(11) NOT NULL DEFAULT '0' COMMENT '0 正常 1停用 -1已删除',
  `createTime` datetime NOT NULL,
  `updateTime` datetime DEFAULT NULL,
  `remark` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`projectId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='项目表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `projectconfig`
--

DROP TABLE IF EXISTS `projectconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `projectconfig` (
  `projectId` int(11) NOT NULL,
  `configKey` varchar(100) NOT NULL,
  `configValue` varchar(200) NOT NULL DEFAULT '',
  `remark` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`projectId`,`configKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='项目的配置';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `projectversion`
--

DROP TABLE IF EXISTS `projectversion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `projectversion` (
  `versionId` int(11) NOT NULL AUTO_INCREMENT,
  `projectId` int(11) NOT NULL,
  `versionNo` varchar(60) NOT NULL,
  `createTime` datetime NOT NULL,
  `versionInfo` varchar(2000) NOT NULL DEFAULT '' COMMENT '版本信息',
  `downloadUrl` varchar(400) NOT NULL DEFAULT '',
  `remark` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`versionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='项目版本';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serverconfig`
--

DROP TABLE IF EXISTS `serverconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `serverconfig` (
  `serverId` int(11) NOT NULL,
  `configKey` varchar(50) NOT NULL,
  `configValue` varchar(500) NOT NULL DEFAULT '',
  `remark` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`serverId`,`configKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `servermachine`
--

DROP TABLE IF EXISTS `servermachine`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `servermachine` (
  `serverId` int(11) NOT NULL AUTO_INCREMENT,
  `serverName` varchar(100) NOT NULL DEFAULT '',
  `serverIPs` varchar(200) NOT NULL DEFAULT '' COMMENT '服务器IP,用'',''号分开',
  `serverMACs` varchar(200) NOT NULL DEFAULT '' COMMENT '服务器MAC，用'',''号隔开',
  `clientIds` varchar(200) NOT NULL DEFAULT '' COMMENT '客户端的GUID号，用,号隔开',
  `serverOS` varchar(100) NOT NULL DEFAULT '' COMMENT '服务器操作系统及版本位数',
  `lastHeartTime` datetime DEFAULT NULL,
  `createTime` datetime NOT NULL,
  `updateTime` datetime DEFAULT NULL,
  `configUpdateTime` datetime DEFAULT NULL,
  `serverOfType` int(11) NOT NULL DEFAULT '0' COMMENT '0 公司\n1 客户',
  `serverState` int(11) NOT NULL DEFAULT '0' COMMENT '服务器状态\n0 正常使用中   1备用  2停止服务 -1删除',
  `remark` varchar(200) NOT NULL DEFAULT '',
  `valStartTime` datetime DEFAULT NULL,
  `valEndTime` datetime DEFAULT NULL,
  `configSign` varchar(50) NOT NULL DEFAULT '' COMMENT 'confgmd5',
  `config` text,
  PRIMARY KEY (`serverId`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COMMENT='服务器表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serverproject`
--

DROP TABLE IF EXISTS `serverproject`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `serverproject` (
  `serverProjectId` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(200) NOT NULL DEFAULT '',
  `projectId` int(11) NOT NULL,
  `serverId` int(11) NOT NULL DEFAULT '0',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT '0 正常    1关闭  -1已删除',
  `copyRightConfig` text,
  `tag` varchar(200) NOT NULL DEFAULT '' COMMENT '标签 []',
  `remark` varchar(400) NOT NULL DEFAULT '',
  `functionRemark` varchar(800) NOT NULL DEFAULT '' COMMENT '功能说明',
  `serverVersion` varchar(100) NOT NULL DEFAULT '' COMMENT '服务器上的当前版本',
  `lastUpdateTime` datetime DEFAULT NULL COMMENT '上次版本更新时间',
  `createTime` datetime NOT NULL,
  `updateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`serverProjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='客户项目表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serverprojectconfig`
--

DROP TABLE IF EXISTS `serverprojectconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `serverprojectconfig` (
  `serverProjectId` int(11) NOT NULL,
  `configKey` varchar(50) NOT NULL,
  `projectId` int(11) NOT NULL COMMENT '冗余用 项目id',
  `configValue` varchar(500) NOT NULL DEFAULT '',
  `canDelete` int(11) NOT NULL DEFAULT '0',
  `remark` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`serverProjectId`,`configKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tagpermission`
--

DROP TABLE IF EXISTS `tagpermission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tagpermission` (
  `usertagId` int(11) NOT NULL,
  `permissionKey` varchar(100) NOT NULL,
  PRIMARY KEY (`usertagId`,`permissionKey`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task`
--

DROP TABLE IF EXISTS `task`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `task` (
  `taskId` int(10) NOT NULL AUTO_INCREMENT,
  `codeName` varchar(100) NOT NULL,
  `title` varchar(100) NOT NULL,
  `state` int(10) NOT NULL,
  `createTime` datetime NOT NULL,
  `updateTime` datetime DEFAULT NULL,
  `remark` varchar(200) DEFAULT NULL,
  `severState` int(11) DEFAULT '0' COMMENT '当前任务在服务器运行的状态(0停止1运行)',
  `memory` varchar(200) DEFAULT '' COMMENT '占有内存情况',
  `lastTime` datetime DEFAULT NULL COMMENT '上一次运行时间',
  `taskConfig` varchar(500) DEFAULT NULL COMMENT '任务配置项',
  `serverId` int(10) DEFAULT NULL COMMENT '服务器名称',
  `classFullName` varchar(100) NOT NULL COMMENT '类名',
  `runCron` varchar(100) NOT NULL COMMENT '运行方案',
  `dll` varchar(100) NOT NULL COMMENT '入口DLL',
  `currVersionID` int(11) DEFAULT '0' COMMENT '当前版本',
  `lastHeartTime` datetime DEFAULT NULL,
  PRIMARY KEY (`taskId`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taskversion`
--

DROP TABLE IF EXISTS `taskversion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taskversion` (
  `versionId` int(10) NOT NULL AUTO_INCREMENT,
  `taskId` int(10) NOT NULL,
  `versionNo` varchar(60) NOT NULL,
  `createtime` datetime NOT NULL,
  `versionInfo` varchar(2000) NOT NULL,
  `downloadUrl` varchar(400) NOT NULL,
  `remark` varchar(200) NOT NULL,
  PRIMARY KEY (`versionId`)
) ENGINE=InnoDB AUTO_INCREMENT=70 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `usertag`
--

DROP TABLE IF EXISTS `usertag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `usertag` (
  `usertagId` int(11) NOT NULL AUTO_INCREMENT,
  `tag` varchar(100) NOT NULL,
  `createTime` datetime NOT NULL,
  `remark` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`usertagId`),
  UNIQUE KEY `tag_UNIQUE` (`tag`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workdaily`
--

DROP TABLE IF EXISTS `workdaily`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `workdaily` (
  `workdailyId` int(11) NOT NULL AUTO_INCREMENT,
  `managerid` int(11) NOT NULL,
  `summary` varchar(200) NOT NULL,
  `createTime` datetime NOT NULL,
  `workTime` date NOT NULL,
  `Content` varchar(1000) NOT NULL DEFAULT '',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT '0 正常 -1已删除',
  `score` int(11) NOT NULL DEFAULT '3' COMMENT '1很差 2略低 3正常 4比较好 5非常好',
  PRIMARY KEY (`workdailyId`)
) ENGINE=InnoDB AUTO_INCREMENT=402 DEFAULT CHARSET=utf8 COMMENT='工作日志';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workdistribute`
--

DROP TABLE IF EXISTS `workdistribute`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `workdistribute` (
  `workdistributeId` int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `workitemId` int(11) NOT NULL COMMENT '工作项ID',
  `managerId` int(11) NOT NULL COMMENT '用户ID',
  `state` int(11) NOT NULL COMMENT '完成状态 0 待完成 1已完成',
  `workRemark` text COMMENT '工作备注',
  `actualTime` decimal(10,1) DEFAULT NULL COMMENT '实际用时',
  `commitTime` datetime DEFAULT NULL COMMENT '提交时间',
  PRIMARY KEY (`workdistributeId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COMMENT='工作分配表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workitem`
--

DROP TABLE IF EXISTS `workitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `workitem` (
  `workitemId` int(11) NOT NULL AUTO_INCREMENT COMMENT '工作任务ID',
  `title` varchar(200) NOT NULL COMMENT '工作任务项的标题',
  `content` text COMMENT '工作描述',
  `finalDate` datetime DEFAULT NULL COMMENT '最后日期',
  `createTime` datetime NOT NULL COMMENT '创建时间',
  `updateTime` datetime DEFAULT NULL COMMENT '更新时间',
  `commitTime` datetime DEFAULT NULL COMMENT '提交时间  用时完成时提交到反馈的用',
  `difficulty` int(11) NOT NULL DEFAULT '3' COMMENT '难易度 1很容易 2容易  3一般 4难 5高难度',
  `estimateTime` decimal(10,1) NOT NULL COMMENT '预计用时',
  `actualTime` decimal(10,1) DEFAULT NULL COMMENT '实际用时',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT '状态 0待处理 1处理中 2处理完成 -1已删除',
  `remark` text COMMENT '备注',
  `point` double NOT NULL DEFAULT '0' COMMENT '分值点 0-100',
  `managerId` int(11) NOT NULL COMMENT '管理人ID',
  `managerName` varchar(100) NOT NULL COMMENT '建档人名称',
  `feedbackId` int(11) DEFAULT NULL COMMENT '反馈ID',
  `importance` int(11) NOT NULL DEFAULT '3' COMMENT '重要性 1很低 2低 3一般 4高 5极高',
  `tag` varchar(200) NOT NULL DEFAULT '',
  PRIMARY KEY (`workitemId`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COMMENT='工作任务项';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zhiboprobation`
--

DROP TABLE IF EXISTS `zhiboprobation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `zhiboprobation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `profession` varchar(200) NOT NULL COMMENT '行业',
  `companyNum` int(11) DEFAULT NULL COMMENT '公司人数',
  `mobile` varchar(100) NOT NULL,
  `QQ` varchar(100) DEFAULT NULL,
  `createTime` datetime DEFAULT NULL,
  `remark` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-02-27 13:49:06
