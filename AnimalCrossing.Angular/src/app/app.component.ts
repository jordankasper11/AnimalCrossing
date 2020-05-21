import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { GameService } from './service';
import { Game, GameType, GameMode, GuessRequest, SkipRequest } from './models';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    GameType = GameType;
    GameMode = GameMode;

    game: Game;
    form: FormGroup;
    autoComplete: Array<string>;
    autoCompleteValue: string;

    constructor(private gameService: GameService) {
    }

    async ngOnInit(): Promise<void> {
        const storedType = localStorage.getItem('GameType');
        const type = storedType ? GameType[storedType]: GameType.Villagers;

        const storedMode = localStorage.getItem('GameMode');
        const mode = storedMode ? GameMode[storedMode] : GameMode.Guess;

        await this.newGame(type, mode);
    }

    buildForm(game: Game): FormGroup {
        if (!game.completed) {
            const form = new FormGroup({
                name: new FormControl(null, Validators.required)
            });

            if (game.mode == GameMode.MultipleChoice)
                form.valueChanges.subscribe(async () => await this.submit());
            else {
                const name = form.get('name');

                name.valueChanges
                    .pipe(
                        debounceTime(500),
                        distinctUntilChanged()
                    )
                    .subscribe(async (value: string) => {
                        this.autoComplete = await this.gameService.autoComplete(value.trim()).toPromise();
                        this.autoCompleteValue = value.trim();
                    });
            }

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
            this.hideAutoComplete();

            const request = new GuessRequest(this.game.id, this.form.value.name);
            const response = await this.gameService.guess(request).toPromise();

            this.bindGame(response.game);
        }
    }

    async newGame(type: GameType, mode: GameMode): Promise<void> {
        localStorage.setItem('GameMode', mode.toString());

        const game = await this.gameService.create(type, mode, this.game != null ? this.game.id : null).toPromise();

        this.bindGame(game);
    }

    getAutoCompleteItem(name: string): string {
        const matched = new RegExp('^' + this.autoCompleteValue, "i").exec(name)[0];
        const remaining = name.substring(matched.length);

        return `<strong>${matched}</strong>${remaining}`;
    }

    hideAutoComplete() {
        setTimeout(() => {
            this.autoComplete = null;
            this.autoCompleteValue = null;
        }, 100);
    }

    setName(name: string): void {
        const control = this.form.get('name');

        control.setValue(name);
    }

    private bindGame(game: Game): void {
        this.form = this.buildForm(game);
        this.game = game;

        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
}
