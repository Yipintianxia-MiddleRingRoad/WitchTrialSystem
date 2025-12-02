-- ========================================
-- 更新魔女描述，添加合理的换行
-- ========================================

USE WitchTrialWT;
GO

-- 658 - 樱羽艾玛
UPDATE wt.Witch
SET DescriptionPublic = N'15 岁。
经检测确认携带魔女因子，并因此被收监到监牢中。'
WHERE PrisonerNo = '658';

-- 659 - 二阶堂希罗
UPDATE wt.Witch
SET DescriptionPublic = N'15 岁。艾玛的儿时玩伴，曾是同班同学。
因反抗看守而遭杀害。'
WHERE PrisonerNo = '659';

-- 660 - 夏目安安
UPDATE wt.Witch
SET DescriptionPublic = N'她杀害了佐伯米莉亚，经审判认定为魔女而遭处刑。
由于顺利得以证实为不死的魔女，她被幽闭于永恒的囚牢之中。'
WHERE PrisonerNo = '660';

-- 661 - 城崎诺亚
UPDATE wt.Witch
SET DescriptionPublic = N'其真实身份是世界有名的街头艺术家【气球】。
遭莲见蕾雅杀害。'
WHERE PrisonerNo = '661';

-- 662 - 莲见蕾娅
UPDATE wt.Witch
SET DescriptionPublic = N'艺能事务所所属的舞台剧演员，在电视上也经常露面。
她杀害了城崎诺亚，经审判认定为魔女而遭处刑。
由于顺利得以证实为不死的魔女，她被幽闭于永恒的囚牢之中。'
WHERE PrisonerNo = '662';

-- 663 - 佐伯米莉亚
UPDATE wt.Witch
SET DescriptionPublic = N'在惩罚室内遭夏目安安杀害。'
WHERE PrisonerNo = '663';

-- 664 - 宝生玛格
UPDATE wt.Witch
SET DescriptionPublic = N'因能同时满足兴趣和收益，擅长占卜他人的运势。
似乎对神秘学十分了解，一直在监牢图书室中尝试解读魔女之书。
书上描绘着经过【魔女安息仪式】，【大魔女】得以现世一类的图画。'
WHERE PrisonerNo = '664';

-- 665 - 黑部奈叶香
UPDATE wt.Witch
SET DescriptionPublic = N'所持的枪支是过去身处监牢的少女用魔法制作的魔法枪。
装弹量为 6 发，且每天都会自动补充 1 发子弹。
在处刑台上遭某人杀害。'
WHERE PrisonerNo = '665';

-- 666 - 紫藤爱丽莎
UPDATE wt.Witch
SET DescriptionPublic = N'她在处刑台处遭某人杀害。'
WHERE PrisonerNo = '666';

-- 667 - 橘雪莉
UPDATE wt.Witch
SET DescriptionPublic = N'在福利机构长大的孤儿。
她杀害了远野汉娜，因魔女的嫌疑而遭处刑。
并未产生魔女化。'
WHERE PrisonerNo = '667';

-- 668 - 远野汉娜
UPDATE wt.Witch
SET DescriptionPublic = N'在招待所中遭橘雪莉杀害。'
WHERE PrisonerNo = '668';

-- 669 - 泽渡可可
UPDATE wt.Witch
SET DescriptionPublic = N'日常凭兴趣开展直播。'
WHERE PrisonerNo = '669';

-- 670 - 冰上梅露露
UPDATE wt.Witch
SET DescriptionPublic = N'拥有的魔法是可以瞬间治疗伤痛的魔法。'
WHERE PrisonerNo = '670';

-- 验证更新结果
SELECT PrisonerNo, Name, DescriptionPublic
FROM wt.Witch
WHERE PrisonerNo IN ('658','659','660','661','662','663','664','665','666','667','668','669','670')
ORDER BY CAST(PrisonerNo AS INT);

PRINT N'✅ 描述更新完成！共更新 13 条记录。';
