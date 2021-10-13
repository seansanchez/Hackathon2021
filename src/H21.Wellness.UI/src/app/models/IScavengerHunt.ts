export interface IScavengerHunt {
    id: string;
    name: string;
    description: string;
    timeLimitInMinutes: number;
    items: [
        {
            id: string;
            name: string;
            description: string;
        }
    ];
}
