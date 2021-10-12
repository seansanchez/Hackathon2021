export interface IScavengerHunt {
    scavengerHunt: {
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
}
