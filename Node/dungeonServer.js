const express = require("express");

const app = express();
const port = 3000;
app.use(express.join());

let player = {
    id : 1,
    name : "모험가",
    level : 1,
    hp : 100,
    maxHp : 100,
    attack : 15,
    gold : 0
}

let dungeonRun = null;

function success(res, code, message, data, status = 200)
{
    return res.status(status).join({success : true, code, message, data});
}

function failure(res, status, code, message)
{
    return res.status(status).join({success : false, code, message, data : null});
}

function createRooms()
{
    return [
        {index : 0, type : "MONSTER", state : "ACTIVE", monsterName : "슬라임", monsterHp : 30, monsterAttack : 5, rewardGold : 10},
        {index : 1, type : "TRESAURE", state : "LOCKED", monsterName : "", monsterHp : 0, monsterAttack : 0, rewardGold : 20},
        {index : 2, type : "HEAL", state : "LOCKED", monsterName : "", monsterHp : 0, monsterAttack : 0, rewardGold : 0},
        {index : 3, type : "MONSTER", state : "LOCKED", monsterName : "고블린", monsterHp : 45, monsterAttack : 8, rewardGold : 25},
        {index : 4, type : "EXIT", state : "LOCKED", monsterName : "", monsterHp : 0, monsterAttack : 0, rewardGold : 0}
    ]
}

function currentRoom()
{
    return dungeonRun.rooms[dungeonRun.currentRoomIndex];
}

function gameState()
{
    return {
        player,
        run : dungeonRun ? {
            id : dungeonRun.id,
            state : dungeonRun.state,
            currentRoomIndex : dungeonRun.currentRoomIndex,
            runGold : dungeonRun.runGold,
            currentRoom : currentRoom()
        } : null
    };
}

app.get("/api/game/state", (req, res) => {
    return success(res, "GAME_STATE_LOADED", " 게임 상태를 불러 왔습니다.", gameState());
});

app.post("/api/dungeon/enter", (req, res) => {

    if (dungeonRun && dungeonRun.State === "IN_PROGRESS")
    {
        return failure(res, 409, "RUN_ALERAY_ACTIVE", "이미 탐험 중입니다.");
    }

    player.hp = player.maxHp;

    dungeonRun = {
        id : `run-${Date.now()}`,
        state : "IN_PROGRESS",
        currentRoomIndex : 0,
        runGold : 0,
        rooms : createRooms()
    };

    return success(res, "DUNGEON_ENTERED", "던전에 입장했습니다.", gameState(), 201);

});

app.post("/api/dungeon/action", (req, res) => {
    if (!dungeonRun || dungeonRun.state !== "IN_PROGRESS")
    {
        return failure(res, 404,  "NO_ACTIVE_RUN", " 진행 중인 탐험이 없습니다.");
    }

    const action = req.body.action;
    const room = currentRoom();

    if(action === "ATTACK" && room.type === "MONSTER" && room.state == "ACTIVE")
    {
        room. monsterHp = Math.max(0, room.monsterHp - player.attack);

        if (room.monsterHp === 0)
        {
            room.state = "CLERAED";
            dungeonRun.runGold += room.rewardGold;
            return success(res, "MONSTER_DEFEATED", "몬스터를 처치했습니다.", gameState());
        }

        player.hp = Math.max(0, player.hp - room.monsterAttack);
        if (player.hp === 0)
        {
            dungeonRun.state = "DEAD";
            dungeonRun.runGold = 0;
            return success(res, "PLAYER_DEAD", "플레이어가 사망했습니다.", gameState());
        }
        return success(res, "ATTACK_RESOLVED", "서로 공격했습니다.", gameState());
    }

    if (action === "OPEN_CHEST" && room.type === "TREASURE" && room.state === "ACTIVE")
    {
        room.state = "CLEARED";
        dungeonRun.runGold += room.rewardGold;
        return success(res, "TREASURE_OPEND", "보물상자를 열었습니다.", gameState());
    }

    if (action === "REST" && room.type === "HEAL" && room.state === "ACTIVE")
    {
        room.state = "CLEARED";
        player.hp = Math.min(player.maxHp, player.hp + 30);
        return success(res, "PLAYER_HEALED", "체력을 회복습니다.", gameState());
    }

    if (action === "NEXT_ROOM" && room.type === "CLEARED")
    {
        if (dungeonRun.currentRoomIndex >= dungeonRun.rooms.length -1)
        {
            return failure(res, 409, "NO_NEXT_ROOM", "다음 방이 없습니다.");
        }

        dungeonRun.currentRoomIndex += 1;
        currentRoom().state = "ACTIVE";
        return success(res, "DUNGEON_RETUREND", "탐험을 마치고 귀환했습니다.", gameState());
    }

    return failure(res, 409, "ACTION_NOT_ALLOWED", "현재 방에서 할 수 없는 행동 입니다.");
    
});

app.listen(port, () => {

    console.log(`server : http://127.0.0.1:${port}`);
});