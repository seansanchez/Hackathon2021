export interface IScavengerHunt {
    id: string;
    name: string;
    description: string;
    items: [
        {
            id: string;
            name: string;
            description: string;
        }
    ];
}
