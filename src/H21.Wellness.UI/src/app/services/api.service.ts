import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http'
import { environment } from "src/environments/environment";
import { Observable, timer } from "rxjs";
import { IScavengerHunt } from "../models/IScavengerHunt";
import { IScore } from "../models/IScore";
import { first, map } from "rxjs/operators";
import { IImageConfidence } from "../models/IImageConfidence";

/**
 * A service to interface with the API
 */
@Injectable({
    providedIn: "root"
})
export class ApiService {

    private _apiUrl = environment.apiUrl;

    constructor(private readonly http: HttpClient) { }

    /** Gets a Scavenger Hunt with a specified game code */
    public getScavengerHunt(gameCode: string): Observable<IScavengerHunt> {
        return this.http.get<IScavengerHunt>(`${this._apiUrl}/api/scavenger-hunt/game/${gameCode ? gameCode : 'random'}`);
    }

    /** Gets a calculated score from the API */
    public getScore(gameCode: string, numCompleted: number): Observable<IScore> {
        return this.http.post<IScore>(`${this._apiUrl}/api/scavenger-hunt/score`, <any>{
            id: gameCode,
            completeCount: numCompleted
        });
    }

    /** Gets a calculated score from the API */
    public checkImageMatch(itemId: string, imageUri: string): Observable<IImageConfidence> {
        // return this.http.post<any>(`${this._apiUrl}/api/scavenger-hunt/vision`, {
        //     id: itemId,
        //     imageData: imageUri
        // });
        return timer(1000).pipe(
            first(),
            map(() => <IImageConfidence>{
                id: itemId,
                pass: true,
                confidence: 0.95
            })
        );
    }
}
