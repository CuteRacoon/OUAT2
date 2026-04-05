== girl_thoughts_1 ==
Как темно стало... Нужно зажечь лампу#wait:5
-> DONE

== girl_thoughts_2 ==
Всемил, где же ты? Один в этом лесу...#wait:4
Наверное, напуган#wait:2
Лампа почти погасла
Я должна найти грибы, они поддержат огонь#wait:4
-> DONE

== girl_thoughts_3 ===
Что это было?
->END

== girl_thoughts_4 ===
Так страшно.. Будто преследует кто-то#wait:3
Надо спрататься#wait:2
->END


VAR politeness = 0
== bake_dialogue ==
#othersLine_4
Здравствуй девочка, куда путь держишь?
* [Здравствуй, Печка. Не видела ли ты, куда гуси-лебеди моего братца унесли?] 
    ~ politeness = politeness + 0
    -> dont_see_1
* [Печка-Печка! Куда гуси-лебеди унесли моего братца?]
    ~ politeness = politeness + 1
    -> dont_see_2

== dont_see_1 ==
#othersLine_4
Спала я, девочка, и не видала никаких гусей
-> girl_asks_1

== dont_see_2 ==
#othersLine_4
Не видала я никаких гусей.
-> girl_asks_1

== girl_asks_1 ==
* [Простите, что потревожила ваш сон. Неужели я не смогу найти своего брата?..]
    ~ politeness = politeness + 0
    -> cannot_find
* [Ох как же так. Неужели ты совсем ничего не видела?]
    ~ politeness = politeness + 1
    -> cannot_find

== cannot_find ==
#othersLine_4
Не видела я гусей, но могу сказать, где они живут, коли отведаешь моих пирожков
* [Спасибо, вам Печка, за угощение. Отведаю ваших пирожков]
    ~ politeness = politeness + 0
    -> check_politeness
* [Спасибо, вам Печка, за угощение. Отведала бы, да не голодна я]
    ~ politeness = politeness + 1
    -> check_politeness
* [Не буду я есть никаких пирожков]
    ~ politeness = politeness + 2
    -> check_politeness
    
== check_politeness ==
{ politeness == 0:
    -> polite_branch_1
- else:
    { politeness < 2:
        -> medium_branch_1
      - else:
        -> rude_branch_1
    }
}
== polite_branch_1 ==
#othersLine_4
Ой понравилась ты мне, внученька. В избушку бабы Яги гуси-лебеди твоего братца унесли. Ступай прямо по тропинке
#unlock_friend_of_oven
-> girl_asks_2

== medium_branch_1 ==
#othersLine_4
Ладно уж, скажу тебе кое-что. В избушку бабы Яги гуси-лебеди твоего братца унесли
-> girl_asks_2

== rude_branch_1 ==
#othersLine_4
Тогда ищи своего брата сама, с таким нравом тебе никто не поможет
-> DONE

== girl_asks_2 ==
* [Спасибо, Печка, на забуду вашу доброту]
    -> final_bake_answer_1
* [...]
    ~ politeness = politeness + 1
    -> DONE
    
== final_bake_answer_1 ==
#othersLine_4
Знаешь ли, внученька, что отведав моих пирожков, из лесу уже не вернешься? Только гуси-лебеди границу леса свободно пересекают. А коли найдёшь пёрышко, ими уроненное, да в ладонь зажмёшь — Лес, может статься, тебя и отпустит#wait:5
-> DONE
