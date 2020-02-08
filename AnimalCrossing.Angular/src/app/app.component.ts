import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { GameService } from './service';
import { Game, GameMode, GuessRequest, SkipRequest } from './models';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    GameMode = GameMode;

    game: Game;
    form: FormGroup;

    constructor(private gameService: GameService) {
    }

    async ngOnInit(): Promise<void> {
        await this.newGame();
    }

    buildForm(game: Game): FormGroup {
        if (!game.completed) {
            const form = new FormGroup({
                id: new FormControl(game.id, Validators.required),
                name: new FormControl(null, Validators.required)
            });

            if (game.mode == GameMode.MultipleChoice)
                form.valueChanges.subscribe(async () => await this.submit());

            return form;
        }
        else
            this.form = null;
    }

    async skip(): Promise<void> {
        const request = new SkipRequest(this.game.id);
        const game = await this.gameService.skip(request).toPromise();

        this.bindGame(game);
    }

    async submit(): Promise<void> {
        if (this.form.valid) {
            const request = new GuessRequest(this.form.value.id, this.form.value.name);
            const response = await this.gameService.guess(request).toPromise();

            this.bindGame(response.game);
        }
    }

    async newGame(): Promise<void> {
        this.game = null;

        const game = await this.gameService.create(GameMode.MultipleChoice).toPromise();

        this.bindGame(game);
    }

    private bindGame(game: Game): void {
        this.form = this.buildForm(game);
        this.game = game;
    }
}
