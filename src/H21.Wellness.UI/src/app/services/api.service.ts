import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http'
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { IScavengerHunt } from "../models/IScavengerHunt";
import { IScore } from "../models/IScore";
import { IImageConfidence } from "../models/IImageConfidence";
import { IItemsResponse } from "../models/IItemsResponse";
import { INewScavengerHunt } from "../models/INewScavengerHunt";

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
    public getScore(gameCode: string, numCompleted: number, timeToComplete: number): Observable<IScore> {
        return this.http.post<IScore>(`${this._apiUrl}/api/scavenger-hunt/score`, <any>{
            id: gameCode,
            completeCount: numCompleted,
            completedTimeInSeconds: timeToComplete
        });
    }

    /** Gets a calculated score from the API */
    public checkImageMatch(itemId: string, imageUri: string): Observable<IImageConfidence> {
        return this.http.post<IImageConfidence>(`${this._apiUrl}/api/scavenger-hunt/validate`, {
            id: itemId,
            imageDataUri: imageUri
        });
    }

    /** Gets all items from API */
    public getAllItems(): Observable<IItemsResponse> {
        return this.http.get<IItemsResponse>(`${this._apiUrl}/api/scavenger-hunt/item`);
    }

    /** Gets all items from API */
    public saveScavengerHunt(hunt: INewScavengerHunt): Observable<{ id: string }> {
        return this.http.post<{ id: string }>(`${this._apiUrl}/api/scavenger-hunt`, hunt);
    }
}
