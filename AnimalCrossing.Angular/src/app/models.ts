export class Villager {
    public id: string;
    public name: string;
    public species: string;
    public gender: string;
    public personality: string;
    public catchPhrase: string;
    public imageFileName: string;
    public houseFileName: string;
}

export class CurrentVillager {
    public id: string;
    public imageFileName: string;
}

export class VillagerOption {
    public id: string;
    public name: string;
}

export enum GameType {
    Villagers,
    Houses
}

export enum GameMode {
    Guess,
    MultipleChoice
}

export class Game {
    public id: string;
    public type: GameType;
    public mode: GameMode;
    public completed: boolean;
    public correctGuesses: number;
    public wrongGuesses: number;
    public skips: number;
    public remaining: number;
    public previousVillager: Villager;
    public currentVillager: CurrentVillager;
    public options: Array<VillagerOption>;
}

export class GuessRequest {
    constructor(public gameId: string, public name: string) {
    }
}

export class GuessResponse {
    public success: boolean;
    public game: Game;
}

export class SkipRequest {
    constructor(public gameId: string) {
    }
}