[chs startActionId="1"]
    [line line="夜比想象中深，冷光灯刺眼，皮带套紧伊普洛的手，内圈刺着他的皮肤，他被注射了药剂进 入了昏迷。" bg="undergroundLab" bgm src="Sandbox External Music_Air Apparent_All By Yourself (ft. Krista Youngs and Julia Ross) - Instrumental_(Instrumental Version)]
    [line line="梦比记忆中温暖，阳光在皮肤上跟情人的亲吻一起逡巡"]
    [line line="他确实受够了西城的阴冷，喝多少热 茶跟酒都暖不回僵硬的四肢，双手扶在情人卢克的脸上，热意渗进血管"]
    [line line="就为了这个，他曾经 想过跟他一起走"]
    [line line="如果不是撞见情人私下里还是别人的情人"]
    [line line="情人的吻凑到了他的脖颈间，牙齿跟舌头纠缠厮磨，股间顶着勃起"]
    [line line="情人伏在他身上，温柔地深入"]
    [action id="1" nextActionId="3" roundId="1"]
        [bg="undergroundLab" layer="Background1"]
        [fg="主角全身1" layer="Foreground1"]
        [bgm src="Sandbox External Music_Air Apparent_All By Yourself (ft. Krista Youngs and Julia Ross) - Instrumental_(Instrumental Version)" volume="90" loop action="play"]
        [line actor="朋也" line="你去了哪里，等了你这么久" fsize="15" linespacing="15" fcolor="0x6079d2ff" fstyle="Italic"]
        [line actor="女の子" line="我......" fsize="15" linespacing="15" fcolor="0x6079d2ff" fstyle="Italic"]
        [line line="伊普洛内心迟疑，他们俩从来不谈这些，这是心照不宣的共识。"]
    [action]
    [action id="4" nextActionId="5" previousActionId="3" roundId="1"]
        [bg="undergroundLab" layer="Background1"]
        [fg="少爷1" layer="Foreground1"]
    [action]
    [action id="5" nextActionId="7" previousActionId="4" roundId="1"]
        [bgm src="Sandbox External Music_ELYAZ_The Place Where We Belong_(Original)" loop action="play"]
    [action]
    [action id="7" nextActionId="8" previousActionId="5" roundId="1"]
        [select id="selector-1" type="horizontal"]
            [option text="Choice 1"]
                [line actor="朋也" line="from the male side"]
            [option]
            [option text="Choice 2"]
                [line actor="女の子" line="from the female side"]
            [option]
        [select]
    [action]
    [action id="8" previousActionId="7" roundId="1"]
        [judge id="judge-1"]
            [event evtid="10000"]
            [event]
            [group id="group-1"]
                [pair name="HP" value="15"]
                [pair name="MP" value="10"]
            [group]
        [judge]
    [action]
[che]
